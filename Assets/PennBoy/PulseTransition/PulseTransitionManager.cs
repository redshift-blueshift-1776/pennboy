using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PulseTransitionManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> channels;

    private List<CanvasGroup> cgList;

    private void Awake() {
        cgList = new List<CanvasGroup>();

        foreach (var cg in channels.Select(channel => channel.GetComponent<CanvasGroup>())) {
            cg.alpha = 0f;
            cgList.Add(cg);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator Start() {
        const float firstWaitDuration = 0.07f;
        const float secondWaitDuration = 0.1f;

        const float firstDuration = 0.6f;
        const float secondDuration = 0.7f;

        StartCoroutine(ChangeAlphaTo(cgList[0], 0f, 1f, firstDuration));
        yield return new WaitForSeconds(firstWaitDuration);

        StartCoroutine(ChangeAlphaTo(cgList[1], 0f, 1f, firstDuration));
        StartCoroutine(ChangeAlphaTo(cgList[4], 0f, 1f, firstDuration));
        yield return new WaitForSeconds(firstWaitDuration);

        StartCoroutine(ChangeAlphaTo(cgList[2], 0f, 1f, firstDuration));
        StartCoroutine(ChangeAlphaTo(cgList[5], 0f, 1f, firstDuration));
        yield return new WaitForSeconds(firstWaitDuration);

        StartCoroutine(ChangeAlphaTo(cgList[3], 0f, 1f, firstDuration));
        StartCoroutine(ChangeAlphaTo(cgList[6], 0f, 1f, firstDuration));
        yield return new WaitForSeconds(firstWaitDuration);

        StartCoroutine(ChangeAlphaTo(cgList[7], 0f, 1f, firstDuration));

        // Wait a bit before fading back out
        yield return new WaitForSeconds(0.8f);

        StartCoroutine(ChangeAlphaTo(cgList[0], 1f, 0f, secondDuration));
        yield return new WaitForSeconds(secondWaitDuration);

        StartCoroutine(ChangeAlphaTo(cgList[1], 1f, 0f, secondDuration));
        StartCoroutine(ChangeAlphaTo(cgList[4], 1f, 0f, secondDuration));
        yield return new WaitForSeconds(secondWaitDuration);

        StartCoroutine(ChangeAlphaTo(cgList[2], 1f, 0f, secondDuration));
        StartCoroutine(ChangeAlphaTo(cgList[5], 1f, 0f, secondDuration));
        yield return new WaitForSeconds(secondWaitDuration);

        StartCoroutine(ChangeAlphaTo(cgList[3], 1f, 0f, secondDuration));
        StartCoroutine(ChangeAlphaTo(cgList[6], 1f, 0f, secondDuration));
        yield return new WaitForSeconds(secondWaitDuration);

        StartCoroutine(ChangeAlphaTo(cgList[7], 1f, 0f, secondDuration));

        // Wait for the last channel to disappear and then a little bit more
        yield return new WaitForSeconds(secondDuration + 0.6f);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("HomePage");
    }

    private static IEnumerator ChangeAlphaTo(CanvasGroup cg, float from, float to, float duration) {
        var elapsed = 0f;

        while (elapsed < duration) {
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cg.alpha = to;
    }
}