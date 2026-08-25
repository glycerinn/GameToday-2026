using UnityEngine;

[CreateAssetMenu(
    fileName = "NewWave",
    menuName = "Game/Wave"
)]
public class WaveSO : ScriptableObject
{
    [Header("Wave")]
    public int waveNumber;

    [Header("Enemies")]
    public int enemyCount = 5;
}