using System;
using System.Collections;
using System.Collections.Generic;
using PennBoy;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private CanvasGroup secondBackground;
    [SerializeField] private GameObject gamesCanvas;
    [SerializeField] private Canvas transitionCanvas;
    [SerializeField] private RectTransform leftButtonBar;
    [SerializeField] private RectTransform rightButtonBar;
    [SerializeField] private CanvasGroup barDetails;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject startButton;

    [Header("Credits")]
    [SerializeField] private Button creditsBtn;
    [SerializeField] private RectTransform heartIcon;
    [SerializeField] private RectTransform returnIcon;
    [SerializeField] private GameObject spacer;
    [SerializeField] private GameObject roleGroupPrefab;

    [Header("Quitting")]
    [SerializeField] private Button quitBtn;
    [SerializeField] private GameObject screenshot;
    [SerializeField] private Material grayscale;
    [SerializeField] private CanvasGroup ggText;
    [SerializeField] private RectTransform star;

    // Needs to be greater than the total time of FadeTo()
    private const float TIMER_LENGTH = 5f;
    private const float HEART_INIT_Y = 0f;
    private const float HEART_FINAL_Y = -100f;
    private const float RETURN_INIT_Y = 100f;
    private const float RETURN_FINAL_Y = 0f;
    private static readonly Vector2 CREDITS_SPEED = new(0f, 0.45f);

    private static readonly int InterpolationAmountId = Shader.PropertyToID("_Interpolation_Amount");
    private static readonly int GrayscaleId = Shader.PropertyToID("_Grayscale");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

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
    private RectTransform creditsRt;

    private Vector2 lastLoadingRtSizeDelta;
    private Vector2 lastLoadingRtPos;
    private GameChannel currentGameChannel;
    private string currentGameName;
    private string[] currentCredits;
    private string currentSceneNameToLoad;

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

        creditsRt = spacer.GetComponent<RectTransform>();
    }

    private void Start() {
        overlayCoroutine = StartCoroutine(Anim.Animate(1f, t => {
            overlay.alpha = 1 - t;
        }));

        foreach (Transform trans in gamesList.transform) {
            var obj = trans.gameObject;
            var channelComp = obj.GetComponent<GameChannel>();

            var rgTrans = Instantiate(roleGroupPrefab, spacer.transform).transform;
            var roleTMP = rgTrans.GetChild(0).GetComponent<TMP_Text>();
            var namesTMP = rgTrans.GetChild(1).GetComponent<TMP_Text>();

            roleTMP.text = channelComp.name;
            namesTMP.text = string.Join("\n", channelComp.credits);
        }

        var roleGroupTrans = Instantiate(roleGroupPrefab, spacer.transform).transform;
        var role = roleGroupTrans.GetChild(0).GetComponent<TMP_Text>();
        var names = roleGroupTrans.GetChild(1).GetComponent<TMP_Text>();

        role.text = "PennBoy Main UI";
        names.text = "Charles Wang\nSaahil Gupta\nAnthony Ge";

        roleGroupTrans = Instantiate(roleGroupPrefab, spacer.transform).transform;
        role = roleGroupTrans.GetChild(0).GetComponent<TMP_Text>();
        names = roleGroupTrans.GetChild(1).GetComponent<TMP_Text>();

        role.text = "Thanks for playing!";
        names.text = "";

        // Hack to force vertical layout group to update. See https://stackoverflow.com/a/60204026
        LayoutRebuilder.ForceRebuildLayoutImmediate(spacer.GetComponent<RectTransform>());

        if (!FakeCursor.I.IsVisible) {
            FakeCursor.I.FadeIn();
        }
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

        if (creditsOpen) {
            creditsRt.anchoredPosition += CREDITS_SPEED;
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

    private IEnumerator ShowChannelButtons() {
        // Wait a little for the other animations to finish before animating the buttons,
        // otherwise users may think they're clickable before they really are
        yield return new WaitForSecondsRealtime(0.35f);

        var backPosInit = new Vector2(-280f, -300f);
        var backPosFinal = new Vector2(-280f, -50f);
        var backRt = backButton.GetComponent<RectTransform>();
        StartCoroutine(Anim.Animate(0.55f, t => {
            var newT = Easing.EaseOutExpo(t);
            backRt.anchoredPosition = Vector2.Lerp(backPosInit, backPosFinal, newT);
        }));

        yield return new WaitForSecondsRealtime(0.1f);

        var startPosInit = new Vector2(280f, -300f);
        var startPosFinal = new Vector2(280f, -50f);
        var startRt = startButton.GetComponent<RectTransform>();
        StartCoroutine(Anim.Animate(0.55f, t => {
            var newT = Easing.EaseOutExpo(t);
            startRt.anchoredPosition = Vector2.Lerp(startPosInit, startPosFinal, newT);
        }));
    }

    private IEnumerator HideChannelButtons() {
        var startPosInit = new Vector2(280f, -50f);
        var startPosFinal = new Vector2(280f, -300f);
        var startRt = startButton.GetComponent<RectTransform>();
        StartCoroutine(Anim.Animate(0.3f, t => {
            var newT = Easing.EaseInExpo(t);
            startRt.anchoredPosition = Vector2.Lerp(startPosInit, startPosFinal, newT);
        }));

        yield return new WaitForSecondsRealtime(0.1f);

        var backPosInit = new Vector2(-280f, -50f);
        var backPosFinal = new Vector2(-280f, -300f);
        var backRt = backButton.GetComponent<RectTransform>();
        StartCoroutine(Anim.Animate(0.3f, t => {
            var newT = Easing.EaseInExpo(t);
            backRt.anchoredPosition = Vector2.Lerp(backPosInit, backPosFinal, newT);
        }));
    }

    public IEnumerator OpenGameChannel(string sceneName, string currGameName, string[] currCredits, Sprite thumbnail,
                                       Vector2 initialPos, GameChannel gameChannel) {
        currentGameChannel = gameChannel;
        currentSceneNameToLoad = sceneName;
        currentGameName = currGameName;
        currentCredits = currCredits;

        // Set channel to correct initial position
        loadingThumbnail.sprite = thumbnail;
        loadingRt.anchoredPosition = initialPos;
        loadingObj.SetActive(true);

        // Make the cursor appear above everything. Temporary, will be reset when we leave
        transitionCanvas.sortingOrder = 100;
        creditsBtn.interactable = false;
        quitBtn.interactable = false;

        // I am so sorry!
        lastLoadingRtSizeDelta = loadingRt.sizeDelta;
        lastLoadingRtPos = loadingRt.anchoredPosition;
        var loadingRtSizeDeltaFinal = new Vector2(408.45f, 241.97f);
        var loadingRtPosFinal = new Vector2(1408f, -342f);
        var loadingRtScaleFinal = new Vector3(2.1f, 2.1f, 2.1f);
        var loadingOutlineScaleInit = loadingOutlineRt.localScale;

        var leftButtonBarInit = new Vector2(193f, 0f);
        var leftButtonBarFinal = new Vector2(-250f, 0f);
        var rightButtonBarInit = new Vector2(-193f, 0f);
        var rightButtonBarFinal = new Vector2(250f, 0f);

        gamesCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        StartCoroutine(Anim.Animate(0.25f, t => {
            secondBackground.alpha = t;
        }));

        StartCoroutine(ShowChannelButtons());

        scroller.UpdateText(currGameName);
        if (scroller.IsCurrentlyOpen) scroller.Reset();
        scroller.Appear();

        StartCoroutine(Anim.Animate(0.35f, t => {
            var easeInT = Easing.EaseInExpo(t);

            barDetails.alpha = 1f - t;
            leftButtonBar.anchoredPosition = Vector2.Lerp(leftButtonBarInit, leftButtonBarFinal, easeInT);
            rightButtonBar.anchoredPosition = Vector2.Lerp(rightButtonBarInit, rightButtonBarFinal, easeInT);
            loadingOutlineRt.localScale = Vector3.Lerp(loadingOutlineScaleInit, Vector3.zero, Easing.EaseOutExpo(t));
        }));

        // This yield duration should be enough for *all* animations to finish before we enable
        // the back and start buttons. This means no coroutines should be interrupted
        yield return Anim.Animate(0.7f, t => {
            var easeOutT = Easing.EaseOutExpo(t);

            loadingRt.sizeDelta = Vector2.Lerp(lastLoadingRtSizeDelta, loadingRtSizeDeltaFinal, easeOutT);
            loadingRt.anchoredPosition = Vector2.Lerp(lastLoadingRtPos, loadingRtPosFinal, easeOutT);
            loadingRt.localScale = Vector3.Lerp(Vector3.one, loadingRtScaleFinal, easeOutT);
        });

        // Buttons are not interactable by default
        backButton.GetComponent<Button>().interactable = true;
        startButton.GetComponent<Button>().interactable = true;
    }

    private IEnumerator _CloseGameChannel() {
        backButton.GetComponent<Button>().interactable = false;
        startButton.GetComponent<Button>().interactable = false;

        var loadingRtSizeDeltaInit = loadingRt.sizeDelta;
        var loadingRtPosInit = loadingRt.anchoredPosition;
        var loadingRtScaleInit = loadingRt.localScale;

        var leftButtonBarInit = leftButtonBar.anchoredPosition;
        var leftButtonBarFinal = new Vector2(193f, 0f);
        var rightButtonBarInit = rightButtonBar.anchoredPosition;
        var rightButtonBarFinal = new Vector2(-193f, 0f);

        StartCoroutine(HideChannelButtons());
        StartCoroutine(Anim.Animate(0.35f, t => {
            secondBackground.alpha = 1f - t;
        }));

        // Manually simulate pointer exit event because we need the animations to play. This also
        // causes the scroller to disappear
        currentGameChannel.DisableOnPointerExit = false;
        currentGameChannel.OnPointerExit(null);

        yield return Anim.Animate(0.6f, t => {
            var easeOutT = Easing.EaseOutExpo(t);

            barDetails.alpha = t;
            leftButtonBar.anchoredPosition = Vector2.Lerp(leftButtonBarInit, leftButtonBarFinal, easeOutT);
            rightButtonBar.anchoredPosition = Vector2.Lerp(rightButtonBarInit, rightButtonBarFinal, easeOutT);

            loadingRt.sizeDelta = Vector2.Lerp(loadingRtSizeDeltaInit, lastLoadingRtSizeDelta, easeOutT);
            loadingRt.anchoredPosition = Vector2.Lerp(loadingRtPosInit, lastLoadingRtPos, easeOutT);
            loadingRt.localScale = Vector3.Lerp(loadingRtScaleInit, Vector3.one, easeOutT);
        });

        currentGameChannel.canvasGroup.alpha = 1f;
        loadingObj.SetActive(false);

        // Reset transition canvas order after "Current Loading Game" object has been disabled
        transitionCanvas.sortingOrder = 102;
        creditsBtn.interactable = true;
        quitBtn.interactable = true;

        gamesCanvas.GetComponent<GraphicRaycaster>().enabled = true;
    }

    public void CloseGameChannel() => StartCoroutine(_CloseGameChannel());

    private IEnumerator _OpenGame() {
        FakeCursor.I.FadeOut(true);

        var loadingOutlineScaleInit = new Vector3(0.8f, 0.8f, 0.8f);
        loadingOutlineRt.localScale = loadingOutlineScaleInit;

        // Set channel to correct initial position
        loadingRt.anchoredPosition = loadingRt.anchoredPosition;

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
        Array.Sort(currentCredits);
        gameName.GetComponent<TMP_Text>().text = currentGameName;
        gameCredits.GetComponent<TMP_Text>().text = string.Join(", ", currentCredits);

        var gameNameCG = gameName.GetComponent<CanvasGroup>();
        var creditsCG = gameCredits.GetComponent<CanvasGroup>();
        var loadingRtScaleInit = loadingRt.localScale;
        StartCoroutine(Anim.Animate(0.35f, t => {
            overlay.alpha = t;
            pennBoy.alpha = t;
            gameNameCG.alpha = t;
            creditsCG.alpha = t;
            music.volume = Mathf.Lerp(music.volume, 0f, t);
            loadingOutlineImg.color = Color.Lerp(Theme.Up[1], Color.white, t);

            var newT = Easing.EaseOutExpo(t);
            loadingRt.localScale = Vector3.Lerp(loadingRtScaleInit, Vector3.one, newT);
            loadingOutlineRt.localScale = Vector3.Lerp(loadingOutlineScaleInit, Vector3.one, newT);
        }));

        StartCoroutine(Anim.Animate(0.65f, t => {
            var newT = Easing.EaseOutExpo(t);
            loadingRt.sizeDelta = Vector2.Lerp(loadingRtSizeDeltaInit, loadingRtSizeDeltaFinal, newT);
            loadingRt.anchoredPosition = Vector2.Lerp(loadingRtPosInit, loadingRtPosFinal, newT);
            loadingOutlineRt.offsetMin = Vector2.Lerp(loadingOutlineMinInit, loadingOutlineMinFinal, newT);
            loadingOutlineRt.offsetMax = Vector2.Lerp(loadingOutlineMaxInit, loadingOutlineMaxFinal, newT);
        }));

        var op = SceneManager.LoadSceneAsync(currentSceneNameToLoad)!;
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

        op.allowSceneActivation = true;
    }

    public void OpenGame() => StartCoroutine(_OpenGame());

    private IEnumerator AnimateStarEntry() {
        yield return new WaitForSecondsRealtime(0.35f);

        StartCoroutine(Anim.Animate(0.6f, t => {
            if (t <= 0.5f) {
                var newT = Mathf.Clamp01(t / 0.5f);
                star.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Easing.EaseInOutExpo(newT));
            }
            else {
                var newT = Mathf.Clamp01((t - 0.5f) / 0.5f);
                star.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Easing.EaseInOutExpo(newT));
            }
        }, true));
    }

    private IEnumerator AnimateQuit() {
        yield return new WaitForEndOfFrame();

        // Use a render texture to capture the screen in sRGB
        var temp = RenderTexture.GetTemporary(
            Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB
        );
        ScreenCapture.CaptureScreenshotIntoRenderTexture(temp);

        var colorRt = new RenderTexture(temp.descriptor);
        var grayRt = new RenderTexture(temp.descriptor);

        // Vertically flip the render texture on Windows because DirectX stores textures upside down compared to
        // OpenGL/Vulkan. Therefore, this blit shouldn't be necessary on Linux/macOS builds. Also see
        // https://issuetracker.unity3d.com/issues/graphics-capturescreenshotintorendertexture-output-is-flipped-in-the-editor
        if (SystemInfo.graphicsDeviceType.ToString().StartsWith("Direct3D")) {
            Graphics.Blit(temp, colorRt, new Vector2(1f, -1f), new Vector2(0f, 1f));
        }

        RenderTexture.ReleaseTemporary(temp);

        // Create grayscale version of screenshot
        Graphics.Blit(colorRt, grayRt, grayscale);

        var rawImg = screenshot.GetComponent<RawImage>();
        var imgMat = rawImg.material;
        imgMat.SetTexture(ColorId, colorRt);
        imgMat.SetTexture(GrayscaleId, grayRt);

        screenshot.GetComponent<CanvasGroup>().alpha = 1f;
        secondOverlay.alpha = 1f;
        ggText.alpha = 1f;

        // Slowly fade screen to gray
        StartCoroutine(Anim.Animate(0.5f, t => {
            imgMat.SetFloat(InterpolationAmountId, t);
        }, true));

        StartCoroutine(AnimateStarEntry());

        var ssRect = screenshot.GetComponent<RectTransform>();
        var firstFinalScale = new Vector3(1f, 0.01f, 1f);
        yield return Anim.Animate(0.5f, t => {
            ssRect.localScale = Vector3.Lerp(Vector3.one, firstFinalScale, Easing.EaseInExpo(t));
        }, true);

        // Reset it for use next time (this is not a material instance)
        rawImg.material.SetFloat(InterpolationAmountId, 0f);
        rawImg.material = null;
        Destroy(colorRt);
        Destroy(grayRt);

        yield return Anim.Animate(0.3f, t => {
            ssRect.localScale = Vector3.Lerp(firstFinalScale, Vector3.zero, Easing.EaseInExpo(t));
        }, true);

        yield return new WaitForSecondsRealtime(0.25f);
        yield return Anim.FadeOut(0.8f, ggText, true);
    }

    private IEnumerator _Quit() {
        if (overlay != null) StopCoroutine(overlayCoroutine);

        FakeCursor.I.FadeOut(true);

        var initialVolume = music.volume;
        StartCoroutine(Anim.Animate(1f, t => {
            music.volume = Mathf.Lerp(initialVolume, 0f, t);
        }));

        yield return AnimateQuit();

        Application.Quit();
    }

    public void Quit() {
        if (currentlyQuitting) return;

        currentlyQuitting = true;
        StartCoroutine(_Quit());
    }

    private IEnumerator OpenCredits() {
        channelPositions.Clear();
        creditsBtn.interactable = false;
        creditsOpen = true;

        // Disable grid layout component so we can animate channels and button functionality
        gamesList.GetComponent<GridLayoutGroup>().enabled = false;
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

    private IEnumerator ResetCredits() {
        var creditsCG = spacer.GetComponent<CanvasGroup>();

        yield return Anim.Animate(0.1f, t => {
            creditsCG.alpha = 1f - t;
        });

        creditsOpen = false;
        creditsRt.anchoredPosition = new Vector2(creditsRt.anchoredPosition.x, -300f);
        creditsCG.alpha = 1f;
    }

    private IEnumerator CloseCredits() {
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

        StartCoroutine(ResetCredits());

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
        StartCoroutine(creditsOpen ? CloseCredits() : OpenCredits());
    }
}