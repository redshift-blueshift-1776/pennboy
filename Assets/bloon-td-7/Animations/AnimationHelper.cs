using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    public void AnimationFinished() {
        BTD7.GameManager.instance.inAnimation = false;
    }
}
