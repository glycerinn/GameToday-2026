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
}