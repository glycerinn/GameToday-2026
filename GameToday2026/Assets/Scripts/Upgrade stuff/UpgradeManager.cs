using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    [Header("Upgrades")]
    public UpgradeSO[] availableUpgrades;
    private List<UpgradeSO> chosenUpgrades = new List<UpgradeSO>();

    [Header("UI")]
    public GameObject upgradePanel;

    [Header("Player")]
    public GameObject player;

    public Button[] upgradeButtons;
    public TMP_Text[] upgradeNames;
    public TMP_Text[] upgradeDescriptions;
    public Button confirmButton;
    private UpgradeSO[] currentChoices;

    private int selectedIndex = -1;

    public static bool UpgradeSelectionActive { get; private set; }

    void Start()
    {
        upgradePanel.SetActive(false);
        confirmButton.interactable = false;
        UpgradeSelectionActive = false;
    }

    public void ShowUpgradeChoices()
    {
        UpgradeSelectionActive = true;
        upgradePanel.SetActive(true);
        selectedIndex = -1;
        confirmButton.interactable = false;

        currentChoices = GetRandomUpgrades(3);

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < currentChoices.Length)
            {
                int index = i;

                upgradeButtons[i].gameObject.SetActive(true);
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => SelectUpgrade(index));

                upgradeNames[i].text = currentChoices[i].UpgradeName;
                upgradeDescriptions[i].text = currentChoices[i].description;
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }

        UpdateSelectionVisuals();
    }

    void SelectUpgrade(int index)
    {
        selectedIndex = index;
        confirmButton.interactable = true;

        UpdateSelectionVisuals();

        Debug.Log("Selected Upgrade: " + currentChoices[index].UpgradeName);
    }

    void UpdateSelectionVisuals()
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            ColorBlock colors = upgradeButtons[i].colors;

            if (i == selectedIndex)
            {
                colors.normalColor = Color.green;
                colors.highlightedColor = Color.green;
                colors.pressedColor = Color.green;
                colors.selectedColor = Color.green;
            }
            else
            {
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                colors.pressedColor = Color.white;
                colors.selectedColor = Color.white;
            }

            upgradeButtons[i].colors = colors;
        }
    }

    public void ConfirmSelection()
    {
        if (selectedIndex < 0)
            return;

        UpgradeSO selectedUpgrade = currentChoices[selectedIndex];

        Debug.Log("CONFIRMED UPGRADE: " + selectedUpgrade.UpgradeName);

        chosenUpgrades.Add(selectedUpgrade);
        ApplyUpgrade(selectedUpgrade);

        UpgradeSelectionActive = false;
        upgradePanel.SetActive(false);

        if (WaveManager.Instance != null)
            WaveManager.Instance.UpgradeSelected();
    }

    void ApplyUpgrade(UpgradeSO upgrade)
    {
        if (upgrade.effect == null)
            return;

        switch (upgrade.effect.type)
        {
            case UpgradeEffectType.MaxHealth:
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.IncreaseMaxHealth(upgrade.effect.value);
                }

                break;
            case UpgradeEffectType.UnlockMM:

                Gun gun = player.GetComponentInChildren<Gun>();

                if (gun != null)
                {
                    gun.UnlockSecondGunMode();
                }

                break;
            case UpgradeEffectType.GrappleHook:

                GrapplingHook grapplingHook =
                    player.GetComponentInChildren<GrapplingHook>();

                if (grapplingHook != null)
                {
                    grapplingHook.UnlockGrapple();
                }

                break;
            case UpgradeEffectType.Knockback:
                Gun knockbackGun = player.GetComponentInChildren<Gun>();

                if (knockbackGun != null)
                    knockbackGun.IncreaseKnockback(upgrade.effect.value);

                break;

            case UpgradeEffectType.FastCharge:
                Gun chargeGun = player.GetComponentInChildren<Gun>();

                if (chargeGun != null)
                    chargeGun.DecreaseChargeTime(upgrade.effect.value);

                break;
            case UpgradeEffectType.Heal:
                PlayerHealth healPlayer = player.GetComponent<PlayerHealth>();

                if (healPlayer != null)
                    healPlayer.IncreaseKillHeal(upgrade.effect.value);

                break;
        }
    }

    UpgradeSO[] GetRandomUpgrades(int amount)
    {
        List<UpgradeSO> remainingUpgrades = new List<UpgradeSO>();

        foreach (UpgradeSO upgrade in availableUpgrades)
        {
            if (!chosenUpgrades.Contains(upgrade))
                remainingUpgrades.Add(upgrade);
        }

        amount = Mathf.Min(amount, remainingUpgrades.Count);

        UpgradeSO[] choices = new UpgradeSO[amount];

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, remainingUpgrades.Count);
            choices[i] = remainingUpgrades[randomIndex];
            remainingUpgrades.RemoveAt(randomIndex);
        }

        return choices;
    }
    
}