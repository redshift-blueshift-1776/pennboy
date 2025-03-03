using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PennBoy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    private static Pause I;

    [SerializeField] public GameObject pauseCanvas;

    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private GameObject secondOverlay;
    [SerializeField] private string[] lockedScenes;

    private bool isPaused;
    private bool isAnimating;

    private float prevTimeScale;
    private CursorLockMode prevLockState;
    private bool prevCursorVisible;
    private List<(AudioSource audioSrc, float volume)> prevAudio;

    private void Awake() {
        if (I == null) {
            I = this;

            SavePreviousStates();
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (isAnimating) return;

        if (Input.GetKeyDown(KeyCode.Escape) && !IsLockedScene()) {
            TogglePauseGame();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (!isPaused) return;

        overlay.alpha = 0f;

        secondOverlay.GetComponent<CanvasGroup>().alpha = 0f;
        secondOverlay.SetActive(false);

        pauseCanvas.SetActive(false);
        isPaused = false;
    }

    private IEnumerator _TogglePauseGame() {
        isAnimating = true;

        // If currently paused, close it. If unpaused, pause it.
        yield return isPaused ? ClosePauseAnimated() : OpenPause();

        // Do the actual toggle
        isPaused = !isPaused;
        isAnimating = false;
    }

    private void TogglePauseGame() => StartCoroutine(_TogglePauseGame());

    private bool IsLockedScene() {
        // This can be optimized if we cache by current scene name, but since we have so few locked scenes it doesn't matter
        var sceneName = SceneManager.GetActiveScene().name;
        return lockedScenes.Any(locked => locked == sceneName);
    }

    private void LoadPreviousStates() {
        Time.timeScale = prevTimeScale;
        Cursor.lockState = prevLockState;
        Cursor.visible = prevCursorVisible;

        foreach (var (audioSource, volume) in prevAudio) {
            audioSource.volume = volume;
        }
    }

    private void SavePreviousStates() {
        prevTimeScale = Time.timeScale;
        prevLockState = Cursor.lockState;
        prevCursorVisible = Cursor.visible;
        prevAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None)
                    .Select(audioSrc => (audioSrc, audioSrc.volume)).ToList();
    }

    private IEnumerator OpenPause() {
        SavePreviousStates();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var (audioSource, _) in prevAudio) {
            audioSource.volume = 0f;
        }

        pauseCanvas.SetActive(true);
        yield return Anim.Animate(0.4f, t => overlay.alpha = Mathf.Lerp(0f, 0.9f, t));

        // Do this at the end so all the animations can actually play
        Time.timeScale = 0f;
    }

    private IEnumerator ClosePauseAnimated() {
        LoadPreviousStates();

        yield return Anim.Animate(0.4f, t => overlay.alpha = Mathf.Lerp(0.9f, 0f, t));

        pauseCanvas.SetActive(false);
    }

    private IEnumerator ClosePauseImmediate() {
        // Reset everything to initial state, but no need for animations. Don't need to set anything for the cursor
        // because the PulseTransition scene does cursor stuff in Awake() anyway
        Time.timeScale = 1f;

        secondOverlay.SetActive(true);
        var cg = secondOverlay.GetComponent<CanvasGroup>();

        yield return Anim.Animate(0.4f, t => cg.alpha = t);
    }

    private IEnumerator _ReturnToPennBoyMenu() {
        if (isAnimating) yield break;

        yield return ClosePauseImmediate();
        SceneManager.LoadScene("PulseTransition");
    }

    private IEnumerator _ResetSoftware() {
        if (isAnimating) yield break;

        yield return ClosePauseImmediate();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToPennBoyMenu() => StartCoroutine(_ReturnToPennBoyMenu());

    public void ResetSoftware() => StartCoroutine(_ResetSoftware());
}