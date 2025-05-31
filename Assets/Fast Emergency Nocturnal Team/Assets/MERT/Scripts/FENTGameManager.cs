using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class FENTGameManager : MonoBehaviour
{
    public static FENTGameManager Instance { get; private set; }



    public bool HasPassenger { get; set; } = false;
    private int score = 0;
    public float timeRemaining = 120f; // 2 min

    [SerializeField] TextMeshProUGUI TimerTxt;
    [SerializeField] TextMeshProUGUI ScoreTxt;

    [SerializeField] public Transform[] pickupSpots;
    [SerializeField] public Transform[] dropoffSpots;

    public FENTGameManager(TextMeshProUGUI timerTxt, TextMeshProUGUI scoreTxt)
    {
        TimerTxt = timerTxt;
        ScoreTxt = scoreTxt;
    }

    public int Score
    {
        get { return score; }
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Total Score: " + score);
    }

    public void DeductScore(int points)
    {
        score -= points;
        if (score < 0) score = 0;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            //Destroy(gameObject); 
            return; 
        }

        Instance = this;

        //DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            float minutes = Mathf.FloorToInt(timeRemaining / 60);
            float seconds = Mathf.FloorToInt(timeRemaining % 60);

            TimerTxt.text = string.Format("{0:00} : {1:00}", minutes, seconds);

            ScoreTxt.text = "Score: " + $"{score}";
            //Debug.Log(score);
        }
        else
        {
            Debug.Log("Time's up! Game over.");
            // implement game end
            SceneManager.LoadScene(30);
            Destroy(gameObject);
        }
    }
    // temp func
    public void ScorePoints()
    {
        score += 10;
        Debug.Log("Scored! Current score: " + score);
    }
}
