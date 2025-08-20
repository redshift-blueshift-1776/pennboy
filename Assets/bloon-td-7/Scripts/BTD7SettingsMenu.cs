using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BTD7SettingsMenu : MonoBehaviour
{
    [SerializeField] private Toggle easyDifficultyToggle;
    [SerializeField] private Toggle mediumDifficultyToggle;
    [SerializeField] private Toggle hardDifficultyToggle;

    [SerializeField] private Toggle descriptionsToggle;

    private int difficulty;
    private int description;
    // Start is called before the first frame update
    private void Start() {
        // Load saved difficulty, 0 is medium, 1 is hard, -1 is easy.
        int savedDifficulty = PlayerPrefs.GetInt("BTD7Difficulty", 0);
        if (savedDifficulty == 0) {
            easyDifficultyToggle.isOn = false;
            mediumDifficultyToggle.isOn = true;
            hardDifficultyToggle.isOn = false;
        } else if (savedDifficulty == 1) {
            easyDifficultyToggle.isOn = false;
            mediumDifficultyToggle.isOn = false;
            hardDifficultyToggle.isOn = true;
        } else {
            easyDifficultyToggle.isOn = true;
            mediumDifficultyToggle.isOn = false;
            hardDifficultyToggle.isOn = false;
        }
        difficulty = savedDifficulty;

        int savedDescription = PlayerPrefs.GetInt("BTD7Descriptions", 1);
        if (savedDescription == 1) {
            descriptionsToggle.isOn = true;
        } else {
            descriptionsToggle.isOn = false;
        }
        description = savedDescription;

        SetUpToggles();
    }

    // Update is called once per frame
    void Update()
    {
        if (difficulty == 0) {
            easyDifficultyToggle.isOn = false;
            mediumDifficultyToggle.isOn = true;
            hardDifficultyToggle.isOn = false;
        } else if (difficulty == 1) {
            easyDifficultyToggle.isOn = false;
            mediumDifficultyToggle.isOn = false;
            hardDifficultyToggle.isOn = true;
        } else {
            easyDifficultyToggle.isOn = true;
            mediumDifficultyToggle.isOn = false;
            hardDifficultyToggle.isOn = false;
        }

        if (description == 1) {
            descriptionsToggle.isOn = true;
        } else {
            descriptionsToggle.isOn = false;
        }
    }

    void SetUpToggles() {
        easyDifficultyToggle.onValueChanged.AddListener((value) =>
            {if (value) difficulty = -1; PlayerPrefs.SetInt("BTD7Difficulty", difficulty);});
        mediumDifficultyToggle.onValueChanged.AddListener((value) =>
            {if (value) difficulty = 0; PlayerPrefs.SetInt("BTD7Difficulty", difficulty);});
        hardDifficultyToggle.onValueChanged.AddListener((value) =>
            {if (value) difficulty = 1; PlayerPrefs.SetInt("BTD7Difficulty", difficulty);});

        descriptionsToggle.onValueChanged.AddListener((value) =>
            {PlayerPrefs.SetInt("BTD7Descriptions", value ? 1 : 0); description = value ? 1 : 0;});
    }
}
