using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PennBoy;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector2 = UnityEngine.Vector2;

public class Pause : MonoBehaviour
{
    private static Pause I;

    [SerializeField] public GameObject pauseCanvas;

    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private GameObject secondOverlay;
    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private CanvasGroup pennBoyMenuButton;
    [SerializeField] private CanvasGroup resetButton;
    [SerializeField] private string[] lockedScenes;

    private bool isPaused;
    private bool isAnimating;

    private float prevTimeScale;
    private CursorLockMode prevLockState;
    private bool prevCursorVisible;
    private List<(AudioSource audioSrc, float volume)> prevAudio;

    private static readonly Vector2 barInit = new(1920f, 0f);
    private static readonly Vector2 topFinal = new(1920f, 315f);
    private static readonly Vector2 bottomFinal = new(1920f, 210f);

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

        // Setting the canvas to false resets all the UI to their initial state!
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

    private IEnumerator ToggleButton(bool setToActive) {
        if (setToActive) {
            yield return StartCoroutine(Anim.Animate(0.12f, t => {
                pennBoyMenuButton.alpha = t;
                resetButton.alpha = t;
            }, true));

            pennBoyMenuButton.interactable = true;
            resetButton.interactable = true;
        }
        else {
            pennBoyMenuButton.interactable = false;
            resetButton.interactable = false;

            StartCoroutine(Anim.Animate(0.2f, t => {
                pennBoyMenuButton.alpha = 1f - t;
                resetButton.alpha = 1f - t;
            }, true));
        }
    }

    private IEnumerator OpenPause() {
        SavePreviousStates();
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var (audioSource, _) in prevAudio) {
            audioSource.volume = 0f;
        }

        pauseCanvas.SetActive(true);

        StartCoroutine(ToggleButton(true));
        StartCoroutine(Anim.Animate(0.4f,
                                    t => overlay.alpha = Mathf.Lerp(0f, 0.9f, Easing.EaseOutExpo(t)),
                                    true));

        StartCoroutine(Anim.Animate(0.7f, t => {
            var newT = Easing.EaseOutExpo(t);
            topBar.sizeDelta = Vector2.Lerp(barInit, topFinal, newT);
            bottomBar.sizeDelta = Vector2.Lerp(barInit, bottomFinal, newT);
        }, true));

        yield return new WaitForSecondsRealtime(0.4f);
    }

    private IEnumerator ClosePauseAnimated() {
        StartCoroutine(ToggleButton(false));
        StartCoroutine(Anim.Animate(0.4f,
                                    t => overlay.alpha = Mathf.Lerp(0.9f, 0f, Easing.EaseInExpo(t)),
                                    true));

        StartCoroutine(Anim.Animate(0.7f, t => {
            var newT = Easing.EaseOutExpo(t);
            topBar.sizeDelta = Vector2.Lerp(topFinal, barInit, newT);
            bottomBar.sizeDelta = Vector2.Lerp(bottomFinal, barInit, newT);
        }, true));

        yield return new WaitForSecondsRealtime(0.7f);
        pauseCanvas.SetActive(false);

        LoadPreviousStates();
    }

    private IEnumerator ClosePauseImmediate() {
        secondOverlay.SetActive(true);
        var cg = secondOverlay.GetComponent<CanvasGroup>();

        yield return Anim.Animate(0.4f, t => cg.alpha = t, true);
        Time.timeScale = 1f;
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