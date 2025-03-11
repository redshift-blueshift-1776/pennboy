using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ButtonType type;

    private enum ButtonType
    {
        Return,
        Reset
    }

    public void OnPointerEnter(PointerEventData eventData) {
        switch (type) {
        case ButtonType.Return:
            Pause.I.ReturnTextAnim(true);
            break;

        case ButtonType.Reset:
            Pause.I.ResetTextAnim(true);
            break;

        default:
            throw new ArgumentOutOfRangeException();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        switch (type) {
        case ButtonType.Return:
            Pause.I.ReturnTextAnim(false);
            break;

        case ButtonType.Reset:
            Pause.I.ResetTextAnim(false);
            break;

        default:
            throw new ArgumentOutOfRangeException();
        }
    }
}