using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win_Zone : MonoBehaviour
{
    //[SerializeField]
    //Game level;

    [SerializeField]
    GameObject player;

    [SerializeField]
    GameObject winText;

    [SerializeField]
    GameObject restartButton;

    [SerializeField]
    GameObject newButton;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        winText.SetActive(false);
        restartButton.SetActive(false);
        newButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col) {
        if(col.gameObject == player) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            winText.SetActive(true);
            restartButton.SetActive(true);
            newButton.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RestartLevel() {
        SceneManager.LoadScene("Level1Test");
    }

    public void ToSecret() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(34);
    }
}