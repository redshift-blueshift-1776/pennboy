using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradeLevelText;
    private string upgradeLevelString = "Upgrade Level: ";

    [SerializeField]
    private GameObject mouseIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private TowerCard cardUsing;
    private PlacementMode currentMode;
    private List<Tower> towersToSacrifice;
    private int totalCost;
    [SerializeField] private TMP_Text modeText;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private TMP_Text upgradeText;

    private TowerIndicator towerIndicator;

    private Dictionary<string, string> towerDescriptions = new Dictionary<string, string>()
    {
        // Basic Tower (single shots)
        {"Basic0", "Basic Tower"},
        {"Basic1", "Twin Shot: Bullets pierce 1 extra enemy."},
        {"Basic2", "Swift Rounds: Bullets fly much faster."},
        {"Basic3", "Hardened Ammo: Stronger bullets deal more damage."},
        {"Basic4", "Lethal Shots: Massive damage with every hit."},

        // Pew Tower (3-round burst tower)
        {"Pew0", "Pew Tower"},
        {"Pew1", "Piercing Burst: Each bullet pierces an extra enemy."},
        {"Pew2", "Impact Rounds: Double damage per shot."},
        {"Pew3", "Rapid Burst: Fires more bullets per burst."},
        {"Pew4", "Overdrive: Huge bursts, faster cooldown, more power."},

        // Shotgun Tower (spread fire)
        {"Shotgun0", "Shotgun Tower"},
        {"Shotgun1", "Scatter Shot: Bullets pierce extra enemies."},
        {"Shotgun2", "Heavy Shells: Double bullet damage."},
        {"Shotgun3", "Quick Loader: Faster firing speed."},
        {"Shotgun4", "Bullet Storm: Fires five shells at once!"},

        // Machine Gun Tower (continuous fire)
        {"Machine Gun0", "Machine Gun Tower"},
        {"Machine Gun1", "High Velocity: Bullets travel faster."},
        {"Machine Gun2", "Shredding Rounds: Double bullet damage."},
        {"Machine Gun3", "Spinning Barrel: Increased fire rate."},
        {"Machine Gun4", "Gatling Fury: Unleashes 25 bullets per second!"},

        // Ring Tower (8-way radial fire)
        {"Ring0", "Ring Tower"},
        {"Ring1", "Razor Rings: Each ring pierces 2 extra enemies."},
        {"Ring2", "Ring Haste: Fires twice as often."},
        {"Ring3", "Charged Rings: Rings deal extra damage."},
        {"Ring4", "Ring Mastery: Launches 16 empowered rings!"}
    };

    private void Start()
    {
        totalCost = 0;
        mouseIndicator.SetActive(false);
        currentMode = PlacementMode.Selection;
        modeText.text = "Placement Mode";
        towerIndicator = mouseIndicator.GetComponent<TowerIndicator>();
        upgradePanel.SetActive(false);
    }

    private void Update()
    {
        // Toggles sacrificing mode when Q pressed and card can sacrifice
        if (currentMode != PlacementMode.Selection && cardUsing.canSacrifice && Input.GetKeyDown(KeyCode.Q))
        {
            if (currentMode == PlacementMode.PlacingTower)
            {
                currentMode = PlacementMode.Sacrificing;
                modeText.text = "Sacrifice Mode";
                int savedDescription = PlayerPrefs.GetInt("BTD7Descriptions", 1);
                if (savedDescription == 1) {
                    upgradePanel.SetActive(true);
                    upgradeText.text = "CardUsing: " + cardUsing.GetName() + " " + cardUsing.id;
                    if (towerDescriptions.TryGetValue(cardUsing.GetName() + cardUsing.GetLevel(totalCost), out string description)) {
                        upgradeText.text = description;
                    }
                }
            }
            else
            {
                currentMode = PlacementMode.PlacingTower;
                modeText.text = "Placement Mode";
                upgradePanel.SetActive(false);
            }
        }

        switch (currentMode)
        {
            case PlacementMode.Sacrificing:
                towerIndicator.gameObject.SetActive(false);
                // Logic for tower upgrading (sacrificing)
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Clicked");
                    Tower towerClicked = inputManager.GetTowerClicked();
                    if (towerClicked != null)
                    {
                        Debug.Log("Tower Clicked");
                        bool addTower = towerClicked.ToggleSacrifice();
                        if (addTower)
                        {
                            towersToSacrifice.Add(towerClicked);
                            totalCost += towerClicked.GetCost();
                            updateUpgradeLevelText();
                        }
                        else
                        {
                            towersToSacrifice.Remove(towerClicked);
                            totalCost -= towerClicked.GetCost();
                            updateUpgradeLevelText();
                        }
                        
                    }
                }
                return;
            case PlacementMode.PlacingTower:
                towerIndicator.gameObject.SetActive(true);

                upgradeLevelText.gameObject.SetActive(true);
                if (!towersToSacrifice.Any()) {
                    upgradeLevelText.text = upgradeLevelString + 0;
                }

                if (cardManager.MouseHovering) {
                    towerIndicator.gameObject.SetActive(false);
                    return;
                }

                // change range indicator size
                towerIndicator.SetRangeIndicator(cardUsing.GetRange());

                
                // Logic for tower placement
                (Vector3 MousePosition, bool validplacement) = inputManager.GetPlacementPosition();

                mouseIndicator.transform.position = MousePosition + new Vector3(0, 3.5f, 0);
                if (!validplacement) { 
                    towerIndicator.CantPlace();
                    return;
                }
                else {
                    towerIndicator.CanPlace();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    MoneyManager moneyManager = BTD7.GameManager.instance.moneyManager;
                    if (moneyManager.GetCurrentMoney() < cardUsing.GetCost()) { return; }

                    // deduct money
                    moneyManager.SpendMoney(cardUsing.GetCost());

                    // this goes hard
                    Tower t = Instantiate(cardUsing.towerPrefab, MousePosition + new Vector3(0, 3.5f, 0), new Quaternion()).GetComponent<Tower>();
                    if (cardUsing.canSacrifice)
                    {
                        int upgradeAmount = calculateTotalSacrifice();
                        t.CalcLevel(upgradeAmount);
                    }

                    disableTowerPlacement(true);
                }
                return;
            case PlacementMode.Selection:
                // Do logic for selecting towers to open a UI menu here
                upgradeLevelText.gameObject.SetActive(false);
                return;
        }
    }

    private void disableSacrifices() {
        foreach (Tower t in towersToSacrifice) {
            t.ToggleSacrifice();
        }
        towersToSacrifice.Clear();
    }

    private void updateUpgradeLevelText() {
        upgradeLevelText.text = upgradeLevelString + cardUsing.GetLevel(totalCost);
        int savedDescription = PlayerPrefs.GetInt("BTD7Descriptions", 1);
        if (savedDescription == 1) {
            if (towerDescriptions.TryGetValue(cardUsing.GetName() + cardUsing.GetLevel(totalCost), out string description)) {
                upgradeText.text = description;
            }
        }
        
    }

    /// <summary>
    /// Calculates the total cost of all towers being sacrificed. Only to be called when sacrificing towers, so destroys all towers in the process.
    /// </summary>
    /// <returns></returns>
    private int calculateTotalSacrifice()
    {
        int amount = calcCost();
        foreach (Tower t in towersToSacrifice) {
            t.Die();
        }
        towersToSacrifice.Clear();
        return amount;
    }

    private int calcCost() {
        int sum = 0;
        foreach (Tower t in towersToSacrifice) {
            sum += t.GetCost();
        }
        return sum;
    }

    /// <summary>
    /// Enables tower placement for the specified tower
    /// </summary>
    /// <param name="id">id of tower to place</param>
    /// <param name="card">reference to card calling this function</param>
    public void enableTowerPlacement(TowerCard card)
    {
        totalCost = 0;
        if (currentMode != PlacementMode.Selection)
        {
            cardUsing.Deactivate(false);
        }
        cardUsing = card;
        mouseIndicator.SetActive(true);
        currentMode = PlacementMode.PlacingTower;
        if (towersToSacrifice != null)
        {
            foreach (Tower t in towersToSacrifice)
                t.ToggleSacrifice();
        }
        towersToSacrifice = new List<Tower>();
    }

    /// <summary>
    /// Disables tower placement.
    /// </summary>
    /// <param name="didPlace">True if did place the tower, false if exiting on escape</param>
    public void disableTowerPlacement(bool didPlace)
    {
        cardUsing.Deactivate(didPlace);
        mouseIndicator.SetActive(false);
        currentMode = PlacementMode.Selection;
        disableSacrifices();
        modeText.text = "Placement Mode";
    }

    private enum PlacementMode
    {
        Sacrificing,
        PlacingTower,
        Selection
    }
}
