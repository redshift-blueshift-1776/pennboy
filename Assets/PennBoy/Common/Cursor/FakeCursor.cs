using System.Collections;
using PennBoy;
using UnityEngine;

public class FakeCursor : MonoBehaviour
{
    public static FakeCursor I;

    [SerializeField] private Transform group;
    [SerializeField] private RectTransform cursorRect;
    [SerializeField] private CanvasGroup cursorCG;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;

    private float timer;
    private const float tick = 0.04f;
    private static readonly Vector3 initScale = new(0.1566519f, 0.1566519f, 0.1566519f);

    private void Awake() {
        if (I == null) {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        Cursor.visible = false;
    }

    private void Update() {
        var currPosition = Input.mousePosition + new Vector3(offsetX, offsetY, 0f);

        cursorRect.position = currPosition;
        timer += Time.unscaledDeltaTime;

        if (timer > tick) {
            var particle = Instantiate(particlePrefab, group);
            particle.GetComponent<RectTransform>().position = currPosition;
            particle.transform.SetAsFirstSibling();

            StartCoroutine(KillParticle(particle));
            timer = 0f;
        }
    }

    private static IEnumerator KillParticle(GameObject particle) {
        var rt = particle.GetComponent<RectTransform>();
        var cg = particle.GetComponent<CanvasGroup>();

        yield return Anim.Animate(0.4f, t => {
            var newT = Easing.EaseInExpo(t);
            rt.localScale = Vector3.Lerp(initScale, Vector3.zero, newT);
            cg.alpha = 1f - t;
        }, true);

        Destroy(particle);
    }

    private IEnumerator _FadeOut(bool lockCursorAfter) {
        yield return Anim.FadeOut(0.15f, cursorCG, true);
        if (lockCursorAfter) {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void FadeIn() => StartCoroutine(Anim.FadeIn(0.15f, cursorCG, true));
    public void FadeOut(bool lockCursorAfter) => StartCoroutine(_FadeOut(lockCursorAfter));

    public bool IsVisible => cursorCG.alpha == 1.0f;
}