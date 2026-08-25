using UnityEngine;
using TMPro;
using System.Collections;

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
    private int enemiesAlive;

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

        WaveSO wave = waves[currentWaveIndex];
        enemiesAlive = wave.enemyCount;

        enemySpawner.SpawnEnemies(wave.enemyCount);
    }

    public void EnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0)
        {
            StartCoroutine(BeginNextWave());
        }
    }

    IEnumerator BeginNextWave()
    {
        // Wait after the last enemy dies
        yield return new WaitForSeconds(delayBeforeNextWave);

        currentWaveIndex++;

        // Check if there are more waves
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

        // Show NEXT WAVE
        if (nextWaveText != null)
        {
            nextWaveText.text =
                "NEXT WAVE " +
                nextWave.waveNumber;

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