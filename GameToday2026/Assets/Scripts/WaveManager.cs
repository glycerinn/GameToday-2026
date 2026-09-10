using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("References")]
    public EnemySpawner enemySpawner;
    public GameOverPanel GamePanel;
    public UpgradeManager upgradeManager;
    public DialogueRunner dialogueRunner;
    public MapManager mapManager;
    public PlayerReset playerReset;

    [Header("Dialogue")]
    public string introDialogueNode = "GameIntro";
    public static bool DialogueActive { get; private set; }

    [Header("Waves")]
    public WaveSO[] waves;

    [Header("UI")]
    public TextMeshProUGUI nextWaveText;
    public Slider enemySlider;
    public Slider waveSlider;

    [Header("Timing & Spawning")]
    public float delayBeforeUpgrades = 2f;
    public float nextWaveTextDuration = 1.5f;
    public float timeBetweenSpawns = 1.2f; // Jeda waktu (detik) antar kemunculan musuh

    private int currentWaveIndex;
    private int enemiesLeftToSpawn;
    private List<IEnemy> aliveEnemies = new List<IEnemy>();

    private bool changingWave;
    private bool isSpawningWave;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (waveSlider != null)
        {
            waveSlider.minValue = 0;
            waveSlider.maxValue = waves.Length;
            waveSlider.value = 0;
        }
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        DialogueActive = true;
        if (AudioManager.instance != null) AudioManager.instance.playDialogueBGM();

        if (dialogueRunner != null && !string.IsNullOrEmpty(introDialogueNode))
        {
            dialogueRunner.StartDialogue(introDialogueNode);
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        }

        DialogueActive = false;
        if (AudioManager.instance != null) AudioManager.instance.playGameBGM();
        StartWave();
    }

    void StartWave()
    {
        if (PlayerHealth.GameOver) return;
        if (currentWaveIndex >= waves.Length)
        {
            GamePanel.ShowWin();
            return;
        }

        changingWave = false;
        WaveSO wave = waves[currentWaveIndex];
        Debug.Log("STARTING WAVE " + wave.waveNumber);

        aliveEnemies.Clear();

        // Hitung total semua musuh di wave ini untuk UI
        int totalWaveEnemies = 0;
        foreach (var data in wave.enemies)
        {
            totalWaveEnemies += data.amount;
        }
        enemiesLeftToSpawn = totalWaveEnemies;

        if (enemySlider != null)
        {
            enemySlider.minValue = 0;
            enemySlider.maxValue = totalWaveEnemies;
            enemySlider.value = totalWaveEnemies;
        }

        if (waveSlider != null) waveSlider.value = currentWaveIndex;

        // Mulai memunculkan musuh satu per satu
        StartCoroutine(SpawnWaveRoutine(wave));
    }

    IEnumerator SpawnWaveRoutine(WaveSO wave)
    {
        isSpawningWave = true;

        foreach (EnemySpawnData data in wave.enemies)
        {
            for (int i = 0; i < data.amount; i++)
            {
                if (PlayerHealth.GameOver) yield break;

                IEnemy enemy = enemySpawner.SpawnSingleEnemy(data.enemyPrefab);
                if (enemy != null)
                {
                    aliveEnemies.Add(enemy);
                }

                enemiesLeftToSpawn--;

                // Tunggu beberapa detik sebelum musuh berikutnya muncul
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        isSpawningWave = false;

        // Cek jika musuh terakhir dibunuh tepat sebelum antrean spawn selesai
        CheckWaveCompletion();
    }

    public void EnemyDied(IEnemy enemy)
    {
        if (PlayerHealth.GameOver || enemy == null) return;

        if (aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Remove(enemy);
        }

        // UI Slider = jumlah musuh hidup + jumlah musuh yang belum di-spawn
        if (enemySlider != null)
            enemySlider.value = aliveEnemies.Count + enemiesLeftToSpawn;

        CheckWaveCompletion();
    }

    private void CheckWaveCompletion()
    {
        if (aliveEnemies.Count == 0 && !isSpawningWave && !changingWave)
        {
            changingWave = true;

            if (currentWaveIndex >= waves.Length - 1)
            {
                Debug.Log("FINAL WAVE COMPLETE!");
                if (nextWaveText != null) GamePanel.ShowWin();
                return;
            }

            StartCoroutine(BeginUpgradeSelection());
        }
    }

    IEnumerator BeginUpgradeSelection()
    {
        yield return new WaitForSeconds(delayBeforeUpgrades);

        WaveSO wave = waves[currentWaveIndex];

        if (dialogueRunner != null && !string.IsNullOrEmpty(wave.upgradeDialogueNode))
        {
            DialogueActive = true;
            if (AudioManager.instance != null) AudioManager.instance.playDialogueBGM();

            dialogueRunner.StartDialogue(wave.upgradeDialogueNode);
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

            DialogueActive = false;
            if (AudioManager.instance != null) AudioManager.instance.playGameBGM();
        }

        if (upgradeManager != null) upgradeManager.ShowUpgradeChoices();
        else Debug.LogError("UpgradeManager is not assigned!");
    }

    public void UpgradeSelected()
    {
        if (PlayerHealth.GameOver) return;
        if (mapManager != null)
        {
            mapManager.SetRandomMap();
            Transform spawnPoint = mapManager.GetCurrentSpawnPoint();
            if (playerReset != null) playerReset.ResetPlayerPosition(spawnPoint);
        }
        StartCoroutine(ContinueToNextWave());
    }

    IEnumerator ContinueToNextWave()
    {
        currentWaveIndex++;
        if (waveSlider != null) waveSlider.value = currentWaveIndex;

        if (currentWaveIndex >= waves.Length)
        {
            if (nextWaveText != null) GamePanel.ShowWin();
            yield break;
        }

        WaveSO nextWave = waves[currentWaveIndex];
        if (nextWaveText != null)
        {
            nextWaveText.text = "NEXT WAVE " + nextWave.waveNumber;
            nextWaveText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(nextWaveTextDuration);
        if (nextWaveText != null) nextWaveText.gameObject.SetActive(false);

        StartWave();
    }
}