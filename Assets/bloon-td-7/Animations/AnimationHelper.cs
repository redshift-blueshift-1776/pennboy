using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    private BTD7.GameManager g;

    void Start() {
        g = BTD7.GameManager.instance;
    }

    public void SetGameManagerIsHiddenTrue() {
        g.isHidden = true;
    }

    public void SetGameManagerIsHiddenFalse() {
        g.isHidden = false;
    }

    public void AnimationFinished() {
        g.inAnimation = false;
    }
}
