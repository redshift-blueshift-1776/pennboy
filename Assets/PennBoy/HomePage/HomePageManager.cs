using System;
using System.Collections;
using System.Collections.Generic;
using PennBoy;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class HomePageManager : MonoBehaviour
{
    [SerializeField] private RawImage background;
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private CanvasGroup secondOverlay;
    [SerializeField] private GameNameScroller scroller;
    [SerializeField] private GameObject gamesList;
    [SerializeField] private GameObject date;
    [SerializeField] private GameObject time;
    [SerializeField] private AudioSource music;

    [Header("Current Loading Game")]
    [SerializeField] private GameObject loadingObj;
    [SerializeField] private CanvasGroup pennBoy;
    [SerializeField] private GameObject gameName;
    [SerializeField] private GameObject gameCredits;

    [Header("Credits")]
    [SerializeField] private Button creditsBtn;
    [SerializeField] private RectTransform heartIcon;
    [SerializeField] private RectTransform returnIcon;

    private const float TIMER_LENGTH = 5f; // Needs to be greater than the total time of FadeTo()
    private const float HEART_INIT_Y = 0f;
    private const float HEART_FINAL_Y = -100f;
    private const float RETURN_INIT_Y = 100f;
    private const float RETURN_FINAL_Y = 0f;

    private CanvasGroup dateCG;
    private CanvasGroup timeCG;
    private Clock clock;
    private float timerElapsed;
    private RectTransform loadingRt;
    private Image loadingThumbnail;
    private GameObject loadingOutline;
    private RectTransform loadingOutlineRt;
    private Image loadingOutlineImg;
    private Coroutine overlayCoroutine;
    private bool currentlyQuitting;
    private List<float> channelPositions;
    private bool creditsOpen;

    private void Awake() {
        overlay.alpha = 1f;
        channelPositions = new List<float>();

        var now = DateTime.Now;
        date.GetComponent<TMP_Text>().text = $"{now:ddd} {now.Month}/{now.Day}";

        // This way we don't have to manually set it for however many games we have...
        foreach (Transform childTransform in gamesList.transform) {
            childTransform.gameObject.GetComponent<GameChannel>().scroller = scroller;
        }

        dateCG = date.GetComponent<CanvasGroup>();
        timeCG = time.GetComponent<CanvasGroup>();

        clock = time.GetComponent<Clock>();
        clock.SetTime(now);

        loadingRt = loadingObj.GetComponent<RectTransform>();
        loadingThumbnail = loadingObj.transform.Find("Mask/Thumbnail").GetComponent<Image>();

        loadingOutline = loadingObj.transform.Find("Outline").gameObject;
        loadingOutlineRt = loadingOutline.GetComponent<RectTransform>();
        loadingOutlineImg = loadingOutline.GetComponent<Image>();
    }

    private void Start() {
        overlayCoroutine = StartCoroutine(Anim.Animate(1f, t => {
            overlay.alpha = 1 - t;
        }));
    }

    private void Update() {
        var now = DateTime.Now;
        date.GetComponent<TMP_Text>().text = $"{now:ddd} {now.Month}/{now.Day}";
        clock.SetTime(now);

        timerElapsed += Time.deltaTime;
        if (timerElapsed >= TIMER_LENGTH) {
            StartCoroutine(FadeTo(dateCG.alpha == 0f));
            timerElapsed = 0f;
        }
    }

    private IEnumerator FadeTo(bool isDate) {
        var exit = isDate ? timeCG : dateCG;
        var enter = isDate ? dateCG : timeCG;

        yield return Anim.Animate(0.12f, t => {
            exit.alpha = 1 - t;
        });
        yield return Anim.Animate(0.12f, t => {
            enter.alpha = t;
        });
    }

    public IEnumerator OpenGame(string sceneName, string currGameName, string[] currCredits, Sprite thumbnail,
                                Vector2 pos) {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Set channel to correct initial position
        loadingThumbnail.sprite = thumbnail;
        loadingRt.anchoredPosition = pos;
        loadingObj.SetActive(true);

        // I am so sorry
        var loadingRtSizeDeltaInit = loadingRt.sizeDelta;
        var loadingRtSizeDeltaFinal = new Vector2(787.7651f, 466.6801f);
        var loadingRtPosInit = loadingRt.anchoredPosition;
        var loadingRtPosFinal = new Vector2(960f, -539.78f);
        var loadingOutlineMinInit = loadingOutlineRt.offsetMin;
        var loadingOutlineMinFinal = new Vector2(-20f, -20f);
        var loadingOutlineMaxInit = loadingOutlineRt.offsetMax;
        var loadingOutlineMaxFinal = new Vector2(20f, 20f);

        if (overlay != null) StopCoroutine(overlayCoroutine);

        // List contributors in alphabetical order to be fair
        Array.Sort(currCredits);
        gameName.GetComponent<TMP_Text>().text = currGameName;
        gameCredits.GetComponent<TMP_Text>().text = string.Join(", ", currCredits);

        var gameNameCG = gameName.GetComponent<CanvasGroup>();
        var creditsCG = gameCredits.GetComponent<CanvasGroup>();
        StartCoroutine(Anim.Animate(0.35f, t => {
            overlay.alpha = t;
            pennBoy.alpha = t;
            gameNameCG.alpha = t;
            creditsCG.alpha = t;
            music.volume = Mathf.Lerp(music.volume, 0f, t);
            loadingOutlineImg.color = Color.Lerp(Theme.Up[1], Color.white, t);
        }));

        StartCoroutine(Anim.Animate(0.65f, t => {
            var newT = Easing.EaseOutExpo(t);
            loadingRt.sizeDelta = Vector2.Lerp(loadingRtSizeDeltaInit, loadingRtSizeDeltaFinal, newT);
            loadingRt.anchoredPosition = Vector2.Lerp(loadingRtPosInit, loadingRtPosFinal, newT);
            loadingOutlineRt.offsetMin = Vector2.Lerp(loadingOutlineMinInit, loadingOutlineMinFinal, newT);
            loadingOutlineRt.offsetMax = Vector2.Lerp(loadingOutlineMaxInit, loadingOutlineMaxFinal, newT);
        }));

        var op = SceneManager.LoadSceneAsync(sceneName)!;
        op.allowSceneActivation = false;

        yield return new WaitForSeconds(0.3f);

        // Make clones of the outlines to perform the outward echo animation
        var outlineParent = loadingOutline.transform.parent;
        var index = 0;
        foreach (var obj in new[] {
                     Instantiate(loadingOutline, outlineParent),
                     Instantiate(loadingOutline, outlineParent),
                     Instantiate(loadingOutline, outlineParent),
                     Instantiate(loadingOutline, outlineParent)
                 }) {
            var rt = obj.GetComponent<RectTransform>();
            var cg = obj.GetComponent<CanvasGroup>();
            var final = Vector3.one * 4f;
            StartCoroutine(Anim.Animate(4f, t => {
                rt.localScale = Vector3.Lerp(Vector3.one, final, Easing.EaseOutExpo(t));
            }));
            StartCoroutine(Anim.Animate(0.35f, t => {
                cg.alpha = 1f - t;
            }));
            yield return new WaitForSeconds(0.12f + index * 0.04f);
            index++;
        }

        yield return new WaitForSeconds(1f);
        yield return Anim.Animate(0.35f, t => {
            secondOverlay.alpha = t;
        });
        yield return new WaitForSeconds(0.1f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        op.allowSceneActivation = true;
    }

    private IEnumerator _Quit() {
        if (overlay != null) StopCoroutine(overlayCoroutine);

        var initialVolume = music.volume;
        yield return StartCoroutine(Anim.Animate(1f, t => {
            overlay.alpha = t;
            music.volume = Mathf.Lerp(initialVolume, 0f, t);
        }));

        Application.Quit();
    }

    public void Quit() {
        if (currentlyQuitting) return;

        currentlyQuitting = true;
        StartCoroutine(_Quit());
    }

    private IEnumerator _OpenCredits() {
        creditsOpen = true;
        channelPositions.Clear();

        // Disable grid layout component so we can animate channels and button functionality
        gamesList.GetComponent<GridLayoutGroup>().enabled = false;
        creditsBtn.interactable = false;
        foreach (Transform trans in gamesList.transform) {
            trans.gameObject.GetComponent<Button>().interactable = false;
        }

        var heartInit = new Vector2(heartIcon.anchoredPosition.x, HEART_INIT_Y);
        var heartFinal = new Vector2(heartIcon.anchoredPosition.x, HEART_FINAL_Y);
        var returnInit = new Vector2(returnIcon.anchoredPosition.x, RETURN_INIT_Y);
        var returnFinal = new Vector2(returnIcon.anchoredPosition.x, RETURN_FINAL_Y);
        StartCoroutine(Anim.Animate(1.5f, t => {
            t = Easing.EaseInOutExpo(t);
            heartIcon.anchoredPosition = Vector2.Lerp(heartInit, heartFinal, t);
            returnIcon.anchoredPosition = Vector2.Lerp(returnInit, returnFinal, t);
        }));

        foreach (Transform trans in gamesList.transform) {
            var rt = trans.gameObject.GetComponent<RectTransform>();
            var init = rt.anchoredPosition;
            channelPositions.Add(init.y);

            var final = new Vector2(init.x, init.y + 800f);
            StartCoroutine(Anim.Animate(0.3f, t => {
                rt.anchoredPosition = Vector2.Lerp(init, final, Easing.EaseInExpo(t));
            }));

            yield return new WaitForSeconds(0.07f);
        }

        yield return new WaitForSeconds(1f);
        creditsBtn.interactable = true;
    }

    private IEnumerator _CloseCredits() {
        creditsOpen = false;
        creditsBtn.interactable = false;

        var heartInit = new Vector2(heartIcon.anchoredPosition.x, HEART_FINAL_Y);
        var heartFinal = new Vector2(heartIcon.anchoredPosition.x, HEART_INIT_Y);
        var returnInit = new Vector2(returnIcon.anchoredPosition.x, RETURN_FINAL_Y);
        var returnFinal = new Vector2(returnIcon.anchoredPosition.x, RETURN_INIT_Y);
        StartCoroutine(Anim.Animate(1.5f, t => {
            t = Easing.EaseInOutExpo(t);
            heartIcon.anchoredPosition = Vector2.Lerp(heartInit, heartFinal, t);
            returnIcon.anchoredPosition = Vector2.Lerp(returnInit, returnFinal, t);
        }));

        // Bring the channels back in the opposite order
        for (var i = channelPositions.Count - 1; i >= 0; i--) {
            var oldY = channelPositions[i];
            var rt = gamesList.transform.GetChild(i).gameObject.GetComponent<RectTransform>();

            var init = rt.anchoredPosition;
            var final = new Vector2(init.x, oldY);
            StartCoroutine(Anim.Animate(0.3f, t => {
                rt.anchoredPosition = Vector2.Lerp(init, final, Easing.EaseOutExpo(t));
            }));

            yield return new WaitForSeconds(0.07f);
        }

        // Allow the player to select games a little bit quicker...
        yield return new WaitForSeconds(0.43f);
        foreach (Transform trans in gamesList.transform) {
            trans.gameObject.GetComponent<Button>().interactable = true;
        }

        // ...than reenabling the credits button, mostly because we're still waiting on the coroutines to finish.
        yield return new WaitForSeconds(0.43f);
        gamesList.GetComponent<GridLayoutGroup>().enabled = true;
        creditsBtn.interactable = true;
    }

    public void ToggleCredits() {
        StartCoroutine(creditsOpen ? _CloseCredits() : _OpenCredits());
    }
}