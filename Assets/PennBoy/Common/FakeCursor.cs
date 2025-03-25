using PennBoy;
using UnityEngine;

public class FakeCursor : MonoBehaviour
{
    public static FakeCursor I;

    [SerializeField] private RectTransform cursorRect;
    [SerializeField] private CanvasGroup cursorCG;
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;

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
        cursorRect.position = Input.mousePosition + new Vector3(offsetX, offsetY, 0f);
    }

    public void FadeIn() => StartCoroutine(Anim.FadeIn(0.15f, cursorCG, true));
    public void FadeOut() => StartCoroutine(Anim.FadeOut(0.15f, cursorCG, true));

    public bool IsVisible => cursorCG.alpha == 1.0f;
}