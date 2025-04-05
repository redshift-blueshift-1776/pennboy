using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BTD7
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public HealthManager healthManager;
        public MoneyManager moneyManager;
        public WaveManager waveManager;
        public CardManager cardManager;
        public GameObject enemy;

        public GameObject projectile;

        public AudioClip teleportSound;
        public AudioClip explosionSound;
        public AudioClip laserShootSound;
        [SerializeField] public GameObject winScreen;

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
{
        if (winScreen != null)
        {
        winScreen.SetActive(false); // Hide it at game start
        }
}

        public void WinGame()
        {
        Time.timeScale = 0f; // optional: pause the game
        winScreen.SetActive(true);
        }

        public void RestartGame()
        {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GoToMainMenu()
        {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // replace with your menu scene name
        }


    }
}


