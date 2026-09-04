using UnityEngine;

[CreateAssetMenu(
    fileName = "NewMap",
    menuName = "Game/Map"
)]
public class MapSO : ScriptableObject
{
    public float[] tileHeights;
}