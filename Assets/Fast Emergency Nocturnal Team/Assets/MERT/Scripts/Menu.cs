using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
   public void OnPlayButton()
    {
        SceneManager.LoadScene(31);
    }

    public void OnQuitButton()
    {
        // Application.Quit();
        SceneManager.LoadScene(2);
    }
}
