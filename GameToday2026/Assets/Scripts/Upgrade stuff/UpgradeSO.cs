using UnityEngine;

[CreateAssetMenu(
    fileName = "NewUpgrade",
    menuName = "Game/Upgrade"
)]
public class UpgradeSO : ScriptableObject
{
    public string UpgradeName;
    [TextArea(2, 4)]
    public string description;
    public UpgradeEffect effect;
}

[System.Serializable]
public class UpgradeEffect
{
    public UpgradeEffectType type;
    public float value;
}

public enum UpgradeEffectType
{
    MaxHealth
}