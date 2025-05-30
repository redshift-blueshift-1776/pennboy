using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class startButton : MonoBehaviour
{
    public void onClick() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(33);
    }

    public void onOtherClick() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(3);
    }

    public void onOtherOtherClick() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(32);
    }
   
}
