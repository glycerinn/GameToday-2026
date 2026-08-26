using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("References")]
    public EnemySpawner enemySpawner;

    [Header("Waves")]
    public WaveSO[] waves;

    [Header("UI")]
    public TextMeshProUGUI nextWaveText;

    [Header("Timing")]
    public float delayBeforeNextWave = 2f;
    public float nextWaveTextDuration = 1.5f;

    private int currentWaveIndex;

    private List<IEnemy> aliveEnemies = new List<IEnemy>();

    private bool changingWave;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartWave();
    }

    void StartWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("ALL WAVES COMPLETE!");
            return;
        }

        changingWave = false;

        WaveSO wave = waves[currentWaveIndex];

        Debug.Log(
            "STARTING WAVE " +
            wave.waveNumber
        );

        aliveEnemies.Clear();

        List<IEnemy> spawnedEnemies =
            enemySpawner.SpawnEnemies(wave.enemies);

        foreach (IEnemy enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                aliveEnemies.Add(enemy);
            }
        }

        Debug.Log(
            "Tracked enemies: " +
            aliveEnemies.Count
        );
    }

    public void EnemyDied(IEnemy enemy)
    {
        if (!aliveEnemies.Contains(enemy))
        {
            Debug.LogWarning(
                "Enemy died but was not being tracked by WaveManager."
            );

            return;
        }

        aliveEnemies.Remove(enemy);

        Debug.Log(
            "Enemy died. Remaining enemies: " +
            aliveEnemies.Count
        );

        if (aliveEnemies.Count == 0 && !changingWave)
        {
            changingWave = true;

            Debug.Log(
                "ALL ENEMIES IN WAVE " +
                (currentWaveIndex + 1) +
                " ARE DEAD!"
            );

            StartCoroutine(BeginNextWave());
        }
    }

    IEnumerator BeginNextWave()
    {
        if (aliveEnemies.Count > 0)
        {
            changingWave = false;
            yield break;
        }

        yield return new WaitForSeconds(delayBeforeNextWave);

        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            if (nextWaveText != null)
            {
                nextWaveText.text = "ALL WAVES COMPLETE!";
                nextWaveText.gameObject.SetActive(true);
            }

            yield break;
        }

        WaveSO nextWave = waves[currentWaveIndex];

        if (nextWaveText != null)
        {
            nextWaveText.text =
                "NEXT WAVE " + nextWave.waveNumber;

            nextWaveText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(nextWaveTextDuration);

        if (nextWaveText != null)
        {
            nextWaveText.gameObject.SetActive(false);
        }

        StartWave();
    }
}