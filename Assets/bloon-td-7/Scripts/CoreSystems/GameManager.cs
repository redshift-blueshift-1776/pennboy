using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
//using UnityEditor.PackageManager;
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
        [SerializeField] private GameObject modeText;
        [SerializeField] private GameObject cardPanel;
        [SerializeField] private Animator CardPanelAnimator;

        [NonSerialized] public bool inAnimation = false;

        private SoundManager soundManager;

        public AudioSource explosionSource;


        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Awake()
        {
            instance = this;
            soundManager = GetComponent<SoundManager>();
        }

        private void Start() {
            if (winScreen != null) {
                winScreen.SetActive(false); // Hide it at game start
            }
            Time.timeScale = 1f;
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.H) && !inAnimation) {
                CardPanelAnimator.SetTrigger("ChangePanelVisibility");
                inAnimation = true;
            }
            if (Input.GetKeyDown(KeyCode.Q) && Input.GetKeyDown(KeyCode.M)) {
                GoToMainMenu();
            }
            if (Input.GetKeyDown(KeyCode.Q) && Input.GetKeyDown(KeyCode.G)) {
                SceneManager.LoadScene(0);
            }
        }

        public void WinGame()
        {
        Time.timeScale = 0f; // Pause the game
        winScreen.SetActive(true);
        cardPanel.SetActive(false);
        modeText.SetActive(false);
        }

        public void FreeplayMode() {
            Time.timeScale = 1f; // Unpause the game
            winScreen.SetActive(false);
            cardPanel.SetActive(true);
            modeText.SetActive(true);
            waveManager.freeplay = true;
            waveManager.waveIndex = 27;
            soundManager.PlayMusic(1);
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


