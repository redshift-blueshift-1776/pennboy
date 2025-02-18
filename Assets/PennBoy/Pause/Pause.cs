using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    private static Pause I;

    [SerializeField] private GameObject pause;
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private string[] lockedScenes;

    private bool isPaused;
    private float prevTimeScale;
    private CursorLockMode prevLockState;
    private bool prevCursorVisible;
    private List<(AudioSource audioSrc, float volume)> prevAudio;

    private void Awake() {
        if (I == null) {
            I = this;

            prevTimeScale = Time.timeScale;
            prevLockState = Cursor.lockState;
            prevCursorVisible = Cursor.visible;
            prevAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None)
                        .Select(audioSrc => (audioSrc, audioSrc.volume)).ToList();

            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !CheckIfBad()) {
            TogglePauseGame();
        }
    }

    public void TogglePauseGame() {
        if (isPaused) {
            Time.timeScale = prevTimeScale;
            Cursor.lockState = prevLockState;
            Cursor.visible = prevCursorVisible;

            foreach (var (audioSource, volume) in prevAudio) {
                audioSource.volume = volume;
            }
        }
        else {
            prevTimeScale = Time.timeScale;
            prevLockState = Cursor.lockState;
            prevCursorVisible = Cursor.visible;
            prevAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None)
                        .Select(audioSrc => (audioSrc, audioSrc.volume)).ToList();

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            foreach (var (audioSource, _) in prevAudio) {
                audioSource.volume = 0f;
            }
        }

        isPaused = !isPaused;
        pause.SetActive(isPaused);
    }

    public void ResumeGame() {
        isPaused = true;
        TogglePauseGame();
    }

    public void ReturnHome() {
        Cursor.lockState = CursorLockMode.None;
        prevLockState = CursorLockMode.None;
        ResumeGame();

        // Let the PulseTransition scene take us back home
        SceneManager.LoadScene("PulseTransition");
    }

    private bool CheckIfBad() {
        //  This can be optimized if we cache by current scene name, but since we have so few locked scenes it doesn't matter
        var sceneName = SceneManager.GetActiveScene().name;
        return lockedScenes.Any(locked => locked == sceneName);
    }
}