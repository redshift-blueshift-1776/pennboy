using System;
using System.Collections;
using UnityEngine;

namespace PennBoy
{
public static class Anim
{
    public static IEnumerator Animate(float animTime, Action<float> enumerate, bool useUnscaledDelta = false) {
        var elapsedTime = 0f;

        while (elapsedTime <= animTime) {
            var t = elapsedTime / animTime;
            enumerate(t);

            elapsedTime += useUnscaledDelta ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }

        // Make sure animation finishes completely e.g. completely interpolates to 1
        enumerate(1f);
    }

    public static IEnumerator FadeIn(float duration, CanvasGroup cg, bool useUnscaledDelta = false) {
        yield return Animate(duration, t => {
            cg.alpha = t;
        }, useUnscaledDelta);
    }

    public static IEnumerator FadeOut(float duration, CanvasGroup cg, bool useUnscaledDelta = false) {
        yield return Animate(duration, t => {
            cg.alpha = 1f - t;
        }, useUnscaledDelta);
    }
}
}