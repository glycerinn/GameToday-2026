using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StyleMeter : MonoBehaviour
{
    public static StyleMeter Instance;

    [Header("Style Settings")]
    public int maxStylePoints = 30;
    public int currentStylePoints;

    [Header("UI")]
    public Slider styleSlider;
    public TextMeshProUGUI rankText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupUI();
        ResetStyle();
    }

    void SetupUI()
    {
        if (styleSlider != null)
        {
            styleSlider.minValue = 0;
            styleSlider.maxValue = maxStylePoints;
            styleSlider.value = 0;
        }
    }

    public void AddStyle(int amount)
    {
        currentStylePoints += amount;

        currentStylePoints = Mathf.Clamp(
            currentStylePoints,
            0,
            maxStylePoints
        );

        UpdateUI();

        Debug.Log("STYLE POINTS: " + currentStylePoints);
    }

    public void ResetStyle()
    {
        currentStylePoints = 0;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (styleSlider != null)
        {
            styleSlider.value = currentStylePoints;
        }

        if (rankText != null)
        {
            rankText.text = GetRankName();
        }
    }

    public string GetRankName()
    {
        if (currentStylePoints >= 26)
            return "SSATISFACTORY";

        if (currentStylePoints >= 21)
            return "SUPERB";

        if (currentStylePoints >= 16)
            return "ADMIRABLE";

        if (currentStylePoints >= 11)
            return "BENEFICIAL";

        if (currentStylePoints >= 6)
            return "CONCERNING";

        return "DEFECTIVE";
    }

    public float GetDamageMultiplier()
    {
        if (currentStylePoints >= 26)
            return 2f;

        if (currentStylePoints >= 21)
            return 1.75f;

        if (currentStylePoints >= 16)
            return 1.50f;

        if (currentStylePoints >= 11)
            return 1.25f;

        return 1f;
    }

    // +1 Normal Kill
    public void EnemyKilled()
    {
        AddStyle(1);
    }

    // +2 Hammer Kill
    public void EnemyHammered()
    {
        AddStyle(2);
    }

    // +2 Kill while player is airborne
    public void EnemyKilledInAir()
    {
        AddStyle(2);
    }

    // -1 Grapple
    public void EnemyGrappled()
    {
        AddStyle(-1);
    }

    // -2 Player damaged
    public void PlayerDamaged()
    {
        AddStyle(-2);
    }
}