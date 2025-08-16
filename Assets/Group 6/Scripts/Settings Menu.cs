using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    public Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityValueText;
    public static float mouseSensitivity = 1;

    [SerializeField] private int coins;

    [SerializeField] private GameObject FENT;
    [SerializeField] private GameObject LocustDash;
    [SerializeField] private TMP_Text secretCoinsText;

    private void Start() {
        // Load saved sensitivity from PlayerPrefs or default to 1.0 if not set
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);
        sensitivitySlider.value = savedSensitivity;

        // Load coins
        int countCoins = 0;
        for (int i = 0; i < 20; i++) {
            // PlayerPrefs.SetInt("Scene " + i + " Coins", 0);
            countCoins += PlayerPrefs.GetInt("Scene " + i + " Coins", 0);
            Debug.Log("Through Scene " + i + " Coins: " + countCoins);
        }
        coins = countCoins;
        FENT.SetActive(false);
        LocustDash.SetActive(false);
        secretCoinsText.text = "Flags: " + coins;
    }

    public void SetMouseSensitivity(float sensitivity) {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity); // Save to PlayerPrefs
        sensitivityValueText.text = "x" + mouseSensitivity.ToString("0.00");
    }

    public void Update() {
        FENT.SetActive(coins >= 24);
        LocustDash.SetActive(coins >= 12);
        secretCoinsText.text = "Flags: " + coins;
    }
}