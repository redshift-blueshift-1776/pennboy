using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PennBoy
{
public class IntroManager : MonoBehaviour
{
    [Header("Background")]
    public GameObject repeatingBg;

    [Header("HealthSafety")]
    public GameObject healthSafety;
    public List<GameObject> text;
    public PressAnyKeyBehavior pressAnyKey;

    [Header("Logo Sequence")]
    public GameObject logoSequence;
    public GameObject upgradeLogo;
    public TMP_Text versionText;
    public List<GameObject> letters;
    public List<Image> stars;
    public AudioSource jingle;
    public AudioSource bing;

    [Header("Transition")]
    public Image overlay;

    private List<(RectTransform rectTransform, CanvasGroup canvasGroup, Image image)> letterAttrs;
    private List<(RectTransform rectTransform, CanvasGroup canvasGroup)> textAttrs;
    private (RectTransform rectTransform, Image image) bgAttrs;
    private (RectTransform rectTransform, CanvasGroup canvasGroup, Image image) upgradeLogoAttrs;

    private bool polling;

    private void Awake() {
#if UNITY_EDITOR
        healthSafety.SetActive(true);
        logoSequence.SetActive(false);
#endif

        // Get all necessary components
        bgAttrs = (repeatingBg.GetComponent<RectTransform>(), repeatingBg.GetComponent<Image>());
        upgradeLogoAttrs = (upgradeLogo.GetComponent<RectTransform>(), upgradeLogo.GetComponent<CanvasGroup>(),
                            upgradeLogo.GetComponent<Image>());

        letterAttrs = new List<(RectTransform, CanvasGroup, Image)>(letters.Count);
        foreach (var letter in letters) {
            letterAttrs.Add((letter.GetComponent<RectTransform>(),
                             letter.GetComponent<CanvasGroup>(),
                             letter.GetComponent<Image>()));
        }

        textAttrs = new List<(RectTransform, CanvasGroup)>(text.Count);
        foreach (var obj in text) {
            textAttrs.Add((obj.GetComponent<RectTransform>(), obj.GetComponent<CanvasGroup>()));
        }

        // Set everything to their default states
        pressAnyKey.paused = true;
        bgAttrs.image.color = Color.black;
        textAttrs.ForEach(attrs => attrs.canvasGroup.alpha = 0f);
        upgradeLogoAttrs.canvasGroup.alpha = 1f;

        foreach (var (rectTrans, canvasGroup, image) in letterAttrs) {
            canvasGroup.alpha = 0f;
            rectTrans.anchoredPosition = new Vector2(rectTrans.anchoredPosition.x, -700);
            image.color = Theme.Up[7];
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator Start() {
        yield return new WaitForSeconds(0.3f);

        // Zoom background in
        var initScale = bgAttrs.rectTransform.localScale;
        var finalScale = new Vector3(2.5f, 2.5f, 2.5f);
        StartCoroutine(Anim.Animate(0.8f, t => {
            t = Easing.EaseOutExpo(t);
            bgAttrs.rectTransform.localScale = Vector3.Lerp(initScale, finalScale, t);
        }));

        // Fade background color into a dark grey
        var finalBgCol = new Color(0.106f, 0.106f, 0.106f);
        StartCoroutine(Anim.Animate(0.3f, t => {
            t = Easing.EaseOutExpo(t);
            bgAttrs.image.color = Color.Lerp(Color.black, finalBgCol, t);
        }));

        // Animate text coming in
        foreach (var (rectTransform, canvasGroup) in textAttrs) {
            var finalPos = rectTransform.anchoredPosition;
            var initYPos = finalPos.y - 100f;

            StartCoroutine(Anim.Animate(0.7f, t => {
                canvasGroup.alpha = t;
                rectTransform.anchoredPosition =
                    new Vector2(finalPos.x, Mathf.Lerp(initYPos, finalPos.y, Easing.EaseOutExpo(t)));
            }));

            yield return new WaitForSeconds(0.1f);
        }

        polling = true;
        pressAnyKey.paused = false;
    }

    private void Update() {
        if (!polling) return;

        if (Input.anyKey) {
            // Don't activate on mouse clicks (UI says "any key" not mouse after all)
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) {
                return;
            }

            polling = false;
            pressAnyKey.paused = true;
            pressAnyKey.Flash();

            StartCoroutine(AnimateLogoSequence());
        }
    }

    private IEnumerator AnimateLogoSequence() {
        // Fade out all the text
        yield return Anim.Animate(0.6f, t => {
            textAttrs.ForEach(attrs => attrs.canvasGroup.alpha = 1f - t);
        });

        // Zoom out background
        var initBgScale = bgAttrs.rectTransform.localScale;
        var finalBgScale = new Vector3(1.5f, 1.5f, 1.5f);
        StartCoroutine(Anim.Animate(0.9f, t => {
            var newT = Easing.EaseInOutQuint(t);
            bgAttrs.rectTransform.localScale = Vector3.Lerp(initBgScale, finalBgScale, newT);
        }));

        // Fade background from grey to Theme 11 (index 10)
        var initBgCol = bgAttrs.image.color;
        StartCoroutine(Anim.Animate(0.6f, t => {
            bgAttrs.image.color = Color.Lerp(initBgCol, Theme.Up[10], t);
        }));

        healthSafety.SetActive(false);
        logoSequence.SetActive(true);

        // Animate PennBoy logo upwards, letter by letter, fading in
        yield return StartCoroutine(RevealPennBoyLetters(letterAttrs));
        yield return new WaitForSeconds(1.5f);

        foreach (var (_, _, image) in letterAttrs) {
            StartCoroutine(PulsePennBoyLetterColor(image));
            yield return new WaitForSeconds(0.1f);
        }

        bing.Play();
        foreach (var star in stars) {
            StartCoroutine(ScaleStars(star));
            StartCoroutine(RotateStars(star));
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.5f);

        // Nudge up all the PennBoy letters at the same time
        foreach (var (rectTrans, canvasGroup, image) in letterAttrs) {
            var midLetterPos = new Vector2(rectTrans.anchoredPosition.x, rectTrans.anchoredPosition.y);
            var finalLetterPos = new Vector2(rectTrans.anchoredPosition.x, 150);
            StartCoroutine(Anim.Animate(1.5f, t => {
                rectTrans.anchoredPosition = Vector2.Lerp(midLetterPos, finalLetterPos, Easing.EaseOutExpo(t));
            }));
        }

        // Wait a bit before introducing UPGRADE logo
        yield return new WaitForSeconds(0.1f);

        // Zoom in background
        initBgScale = bgAttrs.rectTransform.localScale;
        finalBgScale = new Vector3(2.0f, 2.0f, 2.0f);
        StartCoroutine(Anim.Animate(1f, t => {
            bgAttrs.rectTransform.localScale = Vector3.Lerp(initBgScale, finalBgScale, Easing.EaseOutExpo(t));
        }));

        // Translate in UPGRADE logo and fade its color from 8 (index 7) to 4 (index 3)
        var initLogoPos = upgradeLogoAttrs.rectTransform.anchoredPosition;
        var finalLogoPos = new Vector2(upgradeLogoAttrs.rectTransform.anchoredPosition.x, -250);
        StartCoroutine(Anim.Animate(0.7f, t => {
            upgradeLogoAttrs.rectTransform.anchoredPosition =
                Vector2.Lerp(initLogoPos, finalLogoPos, Easing.EaseOutExpo(t));
        }));
        StartCoroutine(Anim.Animate(1.2f, t => {
            upgradeLogoAttrs.image.color = Color.Lerp(Theme.Up[7], Theme.Up[3], t);
        }));

        yield return new WaitForSeconds(0.1f);

        // Fade in version text
        StartCoroutine(Anim.Animate(1f, t => {
            versionText.color = Color.Lerp(Color.clear, Theme.Up[6], t);
        }));

        // Wait for version text to fade in, then start transition
        yield return new WaitForSeconds(2.0f);
        yield return Anim.Animate(1f, t => {
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, t);
        });

        SceneManager.LoadScene("PulseTransition");
    }

    private IEnumerator ScaleStars(Image star) {
        yield return Anim.Animate(0.4f, t => {
            t = Easing.EaseInOutExpo(t);
            star.rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
        });

        yield return Anim.Animate(0.4f, t => {
            t = Easing.EaseInOutExpo(t);
            star.rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
        });
    }

    private IEnumerator RotateStars(Image star) {
        // It's fine if this takes so long, because it will be scaled to zero before it finishes.
        // We just want a slow rotation speed
        yield return Anim.Animate(4f, t => {
            star.rectTransform.Rotate(0f, 0f, 1f);
        });
    }

    private IEnumerator PulsePennBoyLetterColor(Image image) {
        var initColor = image.color;

        yield return Anim.Animate(0.15f, t => {
            image.color = Color.Lerp(initColor, (Color.white + Theme.Up[3]) / 2, t);
        });

        yield return new WaitForSeconds(0.1f);

        yield return Anim.Animate(0.15f, t => {
            image.color = Color.Lerp((Color.white + Theme.Up[3]) / 2, initColor, t);
        });
    }

    private IEnumerator RevealPennBoyLetters(List<(RectTransform, CanvasGroup, Image)> attrs) {
        jingle.PlayDelayed(0.25f);

        // Fades the letters in and translates them upwards onto the screen
        foreach (var (rectTrans, canvasGroup, image) in attrs) {
            var initPos = rectTrans.anchoredPosition;

            // Translate to center of the screen, will be "pushed up" later by the UPGRADE logo
            var finalPos = new Vector2(rectTrans.anchoredPosition.x, 0);

            StartCoroutine(Anim.Animate(0.25f, t => {
                canvasGroup.alpha = t;
            }));

            StartCoroutine(Anim.Animate(1.25f, t => {
                rectTrans.anchoredPosition = Vector2.Lerp(initPos, finalPos, Easing.EaseInOutExpo(t));
            }));

            StartCoroutine(AnimatePennBoyLetterColor(image));

            yield return new WaitForSeconds(0.20f);
        }
    }

    private IEnumerator AnimatePennBoyLetterColor(Image image) {
        // Colors go from 8 (index 7) -> 6 (index 5) -> 4 (index 3)
        for (var i = 4; i > 2; i--) {
            var idx = i * 2;
            yield return Anim.Animate(0.3f, t => {
                image.color = Color.Lerp(Theme.Up[idx - 1], Theme.Up[idx - 3], t);
            });

            yield return new WaitForSeconds(0.75f);
        }
    }
}
}