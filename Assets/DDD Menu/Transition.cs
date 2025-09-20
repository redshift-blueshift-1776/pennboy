using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    [SerializeField] public GameObject rightWall;
    [SerializeField] public GameObject leftWall;
    [SerializeField] public GameObject topWall;
    [SerializeField] public GameObject bottomWall;
    [SerializeField] public GameObject transitionSound;
    // Start is called before the first frame update
    void Start()
    {
        transitionSound.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LoadBTD7() {
        transitionSound.SetActive(true);
        yield return new WaitForSeconds(2f);
        float duration = 1.5f;
        float elapsed = 0f;
        Vector3 ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
        Vector3 ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
        while (elapsed < duration) {
            float t = elapsed / duration;
            rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, new Vector3(0f, 0f, 0f), t * t * t);
            leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, new Vector3(0f, 0f, 0f), t * t * t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rightWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        leftWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        SceneManager.LoadScene(19);

    }

    public void ToBTD7() {
        StartCoroutine(LoadBTD7());
    }

    public IEnumerator LoadMoveInDay() {
        transitionSound.SetActive(true);
        yield return new WaitForSeconds(2f);
        float duration = 1.5f;
        float elapsed = 0f;
        Vector3 ogRWpos = new Vector3(rightWall.transform.localPosition.x, rightWall.transform.localPosition.y, rightWall.transform.localPosition.z);
        Vector3 ogLWpos = new Vector3(leftWall.transform.localPosition.x, leftWall.transform.localPosition.y, leftWall.transform.localPosition.z);
        Vector3 ogTWpos = new Vector3(topWall.transform.localPosition.x, topWall.transform.localPosition.y, topWall.transform.localPosition.z);
        Vector3 ogBWpos = new Vector3(bottomWall.transform.localPosition.x, bottomWall.transform.localPosition.y, bottomWall.transform.localPosition.z);
        while (elapsed < duration) {
            float t = elapsed / duration;
            rightWall.transform.localPosition = Vector3.Lerp(ogRWpos, new Vector3(0f, 0f, 0f), t * t);
            leftWall.transform.localPosition = Vector3.Lerp(ogLWpos, new Vector3(0f, 0f, 0f), t * t);
            topWall.transform.localPosition = Vector3.Lerp(ogTWpos, new Vector3(0f, 0f, 0f), t * t);
            bottomWall.transform.localPosition = Vector3.Lerp(ogBWpos, new Vector3(0f, 0f, 0f), t * t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rightWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        leftWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        topWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        bottomWall.transform.localPosition = new Vector3(0f, 0f, 0f);
        SceneManager.LoadScene(3);

    }

    public void ToMoveInDay() {
        StartCoroutine(LoadMoveInDay());
    }
}
