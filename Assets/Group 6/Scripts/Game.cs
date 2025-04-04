using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private int initialBudget = 1000;
    [SerializeField] private int maxBudget;
    [SerializeField] private HUD hud;
    [SerializeField] public int secretCoins = 0;
    [SerializeField] private string nextLevel;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // set budget
        maxBudget = 2*initialBudget;
        hud.setBudget(initialBudget, maxBudget);
    }

    // Update is called once per frame
    void Update()
    {
        if (hud.getBudget() <= 0) {
            int returnTo = SceneManager.GetActiveScene().buildIndex;
            PlayerPrefs.SetInt("returnTo", returnTo);
            SceneChanger.lvl_name = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("Loss Scene");
            Cursor.lockState = Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void reduceBudget(int damage) {
        hud.reduceBudget(damage);
    }

    public void addCoin() {
        //Debug.Log("Adding coin in game");
        hud.addCoin();
    }

    public void win() {
        PlayerPrefs.SetInt("Scene " + SceneManager.GetActiveScene().buildIndex + " Score", hud.getBudget());
        PlayerPrefs.SetFloat("Scene " + SceneManager.GetActiveScene().buildIndex + " Time", hud.getTime());
        PlayerPrefs.SetInt("Scene " + SceneManager.GetActiveScene().buildIndex + " Coins", hud.getCoins());
        SceneChanger.lvl_time = new TimeSpan(0, 0, Mathf.FloorToInt(hud.TimerAmount));
        if (nextLevel == "NONE") {
            SceneChanger.lvl_name = null;
        } else {
            SceneChanger.lvl_name = nextLevel;
        }
        SceneManager.LoadScene("Win Screen");
    }
}
