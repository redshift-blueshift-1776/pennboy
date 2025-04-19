using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSetter : MonoBehaviour
{
    [SerializeField] private bool cursorVisibility;
    [SerializeField] private CursorLockMode cursorLockMode;

    void Awake() {
        // set cursor visibility and lockstate
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
        // destroy the object
        Destroy(this.gameObject);
    }
}
