using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.PackageManager;
public class UI : MonoBehaviour
{
     public TMP_Text scoreText;

     public GameObject winningCanvas;

     public GameObject losingCanvas;

     public GameObject play;
     public TMP_Text text;

     public int coins;
     public int stage;
    // Start is called before the first frame update
    void Start()
    {
        coinCounter();
        winningCanvas.SetActive(false);
        losingCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        coinCounter();
        stageText();
    }

    public void coinCounter() {
        scoreText.text = "Coins: " + coins;
    }

    public void stageText() {
        text.text = "Stage: " + stage;
    }

    public void won() {
        // play.SetActive(false);
        winningCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void lose() {
        play.SetActive(false);
        losingCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

}
