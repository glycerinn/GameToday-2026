using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int amount = 1;
}

[CreateAssetMenu(fileName = "NewWave", menuName = "Game/Wave")]
public class WaveSO : ScriptableObject
{
    public int waveNumber;

    public EnemySpawnData[] enemies;
}