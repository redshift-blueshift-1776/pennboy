using System.Collections;
using PennBoy;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameChannel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public GameNameScroller scroller;
    [SerializeField] public string[] credits;

    [SerializeField] private string sceneName;
    [SerializeField] private RectTransform outline;

    [Header("Placeholder Mode")]
    [SerializeField] private bool placeholder;
    [SerializeField] private Image background;
    [SerializeField] private GameObject logo;

    private const float SCALE_INIT = 0.8f;
    private const float SCALE_FINAL = 1f;

    private Coroutine curr;
    private HomePageManager manager;
    private CanvasGroup canvasGroup;

    private enum ScaleAnim
    {
        Expand,
        Shrink
    }

    private void Awake() {
        if (placeholder) {
            GetComponent<Button>().enabled = false;
            background.color = Theme.Up[8];
            logo.SetActive(true);
        }

        manager = FindAnyObjectByType<HomePageManager>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (placeholder) return;

        if (curr != null) StopCoroutine(curr);
        curr = StartCoroutine(AnimateScale(ScaleAnim.Expand));

        scroller.UpdateText(gameObject.name);
        if (scroller.IsCurrentlyOpen) scroller.Reset();
        scroller.Appear();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (placeholder) return;

        if (curr != null) StopCoroutine(curr);
        curr = StartCoroutine(AnimateScale(ScaleAnim.Shrink));

        scroller.Disappear();
    }

    private IEnumerator AnimateScale(ScaleAnim anim) {
        var init = outline.localScale;
        var final = anim == ScaleAnim.Expand ? Vector3.one * SCALE_FINAL : Vector3.one * SCALE_INIT;
        var duration = anim == ScaleAnim.Expand ? 0.15f : 2f;

        var elapsed = 0f;
        while (elapsed < duration) {
            outline.localScale = Vector3.Lerp(init, final, Easing.EaseOutExpo(elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        outline.localScale = final;
    }

    public void Open() {
        if (placeholder) return;

        canvasGroup.alpha = 0f;
        StartCoroutine(manager.OpenGame(sceneName, gameObject.name, credits, background.sprite,
                                        GetComponent<RectTransform>().anchoredPosition));
    }
}