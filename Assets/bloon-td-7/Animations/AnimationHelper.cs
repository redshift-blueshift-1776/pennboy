using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    public void SetGameManagerIsHiddenTrue() {
        BTD7.GameManager.instance.isHidden = true;
    }

    public void SetGameManagerIsHiddenFalse() {
        BTD7.GameManager.instance.isHidden = false;
    }
}
