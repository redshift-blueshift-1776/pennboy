using UnityEngine;
using UnityEngine.UI;

public class EndlessBackground : MonoBehaviour
{
    [SerializeField] private RawImage background;

    private void Update() {
        background.uvRect = new Rect(background.uvRect.x + Time.unscaledDeltaTime * 0.07f, background.uvRect.y,
                                     background.uvRect.width, background.uvRect.height);
    }
}