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

    [Header("Credits")]
    [SerializeField] private Button creditsBtn;
    [SerializeField] private RectTransform heartIcon;
    [SerializeField] private RectTransform returnIcon;
    [SerializeField] private GameObject spacer;
    [SerializeField] private GameObject roleGroupPrefab;

    [Header("Quitting")]
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

    private static readonly int InterpolationAmount = Shader.PropertyToID("_Interpolation_Amount");
    private static readonly int Grayscale = Shader.PropertyToID("_Grayscale");
    private static readonly int Color = Shader.PropertyToID("_Color");

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

    public IEnumerator OpenGame(string sceneName, string currGameName, string[] currCredits, Sprite thumbnail,
                                Vector2 pos) {
        FakeCursor.I.FadeOut(true);

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
            loadingOutlineImg.color = UnityEngine.Color.Lerp(Theme.Up[1], UnityEngine.Color.white, t);
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

        // We assume our game start with a visible cursor. They should be setting it to false themselves
        // if they want so!
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        op.allowSceneActivation = true;
    }

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
        imgMat.SetTexture(Color, colorRt);
        imgMat.SetTexture(Grayscale, grayRt);

        screenshot.GetComponent<CanvasGroup>().alpha = 1f;
        secondOverlay.alpha = 1f;
        ggText.alpha = 1f;

        // Slowly fade screen to gray
        StartCoroutine(Anim.Animate(0.5f, t => {
            imgMat.SetFloat(InterpolationAmount, t);
        }, true));

        StartCoroutine(AnimateStarEntry());

        var ssRect = screenshot.GetComponent<RectTransform>();
        var firstFinalScale = new Vector3(1f, 0.01f, 1f);
        yield return Anim.Animate(0.5f, t => {
            ssRect.localScale = Vector3.Lerp(Vector3.one, firstFinalScale, Easing.EaseInExpo(t));
        }, true);

        // Reset it for use next time (this is not a material instance)
        rawImg.material.SetFloat(InterpolationAmount, 0f);
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