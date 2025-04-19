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

    private TowerIndicator towerIndicator;

    private void Start()
    {
        totalCost = 0;
        mouseIndicator.SetActive(false);
        currentMode = PlacementMode.Selection;
        modeText.text = "Placement Mode";
        towerIndicator = mouseIndicator.GetComponent<TowerIndicator>();
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
            }
            else
            {
                currentMode = PlacementMode.PlacingTower;
                modeText.text = "Placement Mode";
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
                if (!towersToSacrifice.Any()) {
                    upgradeLevelText.gameObject.SetActive(true);
                    upgradeLevelText.text = upgradeLevelString + 0;
                }
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
