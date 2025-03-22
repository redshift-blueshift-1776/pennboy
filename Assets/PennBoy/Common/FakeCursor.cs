using UnityEngine;

public class FakeCursor : MonoBehaviour
{
    public static FakeCursor I;

    [SerializeField] private RectTransform cursor;
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
        cursor.position = Input.mousePosition + new Vector3(offsetX, offsetY, 0f);
    }
}