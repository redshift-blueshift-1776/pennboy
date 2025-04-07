using System.Collections;
using PennBoy;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameChannel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Game Info")]
    [SerializeField] public string[] credits;
    [TextArea(2, 5)] public string description;
    [TextArea(2, 5)] public string controlsInstructions;
    [SerializeField] private string sceneName;

    [Header("Placeholder Mode")]
    [SerializeField] private bool placeholder;
    [SerializeField] private Image background;
    [SerializeField] private GameObject logo;

    [Header("References")]
    [SerializeField] private RectTransform outline;

    public bool DisableOnPointerExit { private get; set; }

    [HideInInspector] public CanvasGroup canvasGroup;
    [HideInInspector] public GameNameScroller scroller;

    private const float SCALE_INIT = 0.8f;
    private const float SCALE_FINAL = 1f;

    private Coroutine curr;
    private HomePageManager manager;
    private CanvasGroup tbCopy;

    private static readonly int BarNumber = Shader.PropertyToID("_Bar_Number");
    private static readonly int BarStrength = Shader.PropertyToID("_Bar_Strength");
    private static readonly int Offset = Shader.PropertyToID("_Offset");
    private static readonly int BarSize = Shader.PropertyToID("_Bar_Size");
    private static readonly int FlickerStrength = Shader.PropertyToID("_Flicker_Strength");

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

        var materialInst = new Material(background.material);
        materialInst.SetFloat(BarNumber, Random.Range(1, 4));
        materialInst.SetFloat(BarStrength, Random.Range(0.05f, 0.17f));
        materialInst.SetFloat(Offset, Random.Range(-2f, 2f));
        materialInst.SetFloat(BarSize, Random.Range(0.4f, 0.6f));
        materialInst.SetFloat(FlickerStrength, 0.1f + Random.Range(0f, 0.2f));
        background.material = materialInst;

        var tb = Instantiate(background.gameObject, logo.transform.parent);
        tb.transform.SetSiblingIndex(1);
        tb.GetComponent<Image>().material = null;

        tbCopy = tb.AddComponent<CanvasGroup>();
        //tbCopy.alpha = 0f;
        tbCopy.alpha = 1f;
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
        if (placeholder || DisableOnPointerExit) return;

        if (curr != null) StopCoroutine(curr);
        curr = StartCoroutine(AnimateScale(ScaleAnim.Shrink));

        scroller.Disappear();
    }

    private IEnumerator AnimateScale(ScaleAnim anim) {
        var duration = anim == ScaleAnim.Expand ? 0.15f : 2f;

        var initScale = outline.localScale;
        var finalScale = anim == ScaleAnim.Expand ? Vector3.one * SCALE_FINAL : Vector3.one * SCALE_INIT;

        StartCoroutine(anim == ScaleAnim.Expand ? Anim.FadeIn(0.12f, tbCopy) : Anim.FadeOut(0.12f, tbCopy));

        yield return Anim.Animate(duration, t => {
            outline.localScale = Vector3.Lerp(initScale, finalScale, Easing.EaseOutExpo(t));
        });
    }

    public void Open() {
        if (placeholder) return;

        canvasGroup.alpha = 0f;
        DisableOnPointerExit = true;
        Pause.I.resetScene = sceneName;

        StartCoroutine(manager.OpenGameChannel(sceneName, gameObject.name, credits, background.sprite,
                                               GetComponent<RectTransform>().anchoredPosition, this));
    }
}