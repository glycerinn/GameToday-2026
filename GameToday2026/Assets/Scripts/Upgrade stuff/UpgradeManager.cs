using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("Upgrades")]
    public UpgradeSO[] availableUpgrades;

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
            int index = i;

            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => SelectUpgrade(index));
            upgradeNames[i].text = currentChoices[i].UpgradeName;
            upgradeDescriptions[i].text = currentChoices[i].description;
        }

        UpdateSelectionVisuals();
    }

    void SelectUpgrade(int index)
    {
        selectedIndex = index;

        confirmButton.interactable = true;

        UpdateSelectionVisuals();

        Debug.Log(
            "Selected Upgrade: " +
            currentChoices[index].UpgradeName
        );
    }

    void UpdateSelectionVisuals()
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            ColorBlock colors =
                upgradeButtons[i].colors;

            if (i == selectedIndex)
            {
                colors.normalColor = Color.green;
            }
            else
            {
                colors.normalColor = Color.white;
            }

            upgradeButtons[i].colors = colors;
        }
    }

    public void ConfirmSelection()
    {
        if (selectedIndex < 0)
            return;

        UpgradeSO selectedUpgrade =
            currentChoices[selectedIndex];

        Debug.Log(
            "CONFIRMED UPGRADE: " +
            selectedUpgrade.UpgradeName
        );

        ApplyUpgrade(selectedUpgrade);

        UpgradeSelectionActive = false;
        upgradePanel.SetActive(false);

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.UpgradeSelected();
        }
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
        }
    }

    UpgradeSO[] GetRandomUpgrades(int amount)
    {
        UpgradeSO[] choices = new UpgradeSO[amount];

        int count = 0;

        while (count < amount)
        {
            UpgradeSO randomUpgrade =
                availableUpgrades[
                    Random.Range(
                        0,
                        availableUpgrades.Length
                    )
                ];

            bool alreadySelected = false;

            for (int i = 0; i < count; i++)
            {
                if (choices[i] == randomUpgrade)
                {
                    alreadySelected = true;
                    break;
                }
            }

            if (!alreadySelected)
            {
                choices[count] = randomUpgrade;
                count++;
            }
        }

        return choices;
    }
}