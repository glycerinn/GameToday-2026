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
    public DialogueCharacter dialogueCharacter;
    public MapManager mapManager;
    public PlayerReset playerReset;

    [Header("Dialogue")]
    public string introDialogueNode = "GameIntro";
    public string introDialogueNodetwo = "Endless";
    public static bool DialogueActive { get; private set; }

    [Header("Waves")]
    public WaveSO[] waves;

    [Header("Endless Wave")]
    public WaveSO endlessWave;

    [Header("UI")]
    public TextMeshProUGUI nextWaveText;
    public Slider enemySlider;
    public Slider waveSlider;

    [Header("Timing & Spawning")]
    public float delayBeforeUpgrades = 2f;
    public float nextWaveTextDuration = 1.5f;
    public float timeBetweenSpawns = 1.2f; // Jeda waktu antar kemunculan

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
        PlayerHealth.ResetGameState();
        UpgradeManager.ResetUpgradeState();

        if (PlayerPrefs.GetInt("Endless", 0) == 1)
        {
            StartCoroutine(StartEndlessGame());
            return;
        }

        if (waveSlider != null)
        {
            waveSlider.minValue = 0;
            waveSlider.maxValue = waves.Length;
            waveSlider.value = 0;
        }

        StartCoroutine(StartGame());
    }

    private IEnumerator StartEndlessGame()
    {
        if (waveSlider != null)
        {
            waveSlider.minValue = 0;
            waveSlider.maxValue = 1;
            waveSlider.value = 1;
        }

        if (upgradeManager != null)
            upgradeManager.ApplySavedUpgrades();

        DialogueActive = true;

        if (AudioManager.instance != null)
            AudioManager.instance.playDialogueBGM();

        if (dialogueRunner != null && !string.IsNullOrEmpty(introDialogueNodetwo))
        {
            dialogueRunner.StartDialogue(introDialogueNodetwo);

            if (dialogueCharacter != null)
                dialogueCharacter.ShowCharacter();

            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

            if (dialogueCharacter != null)
                dialogueCharacter.HideCharacter();
        }

        DialogueActive = false;

        if (AudioManager.instance != null)
            AudioManager.instance.playGameBGM();

        StartEndlessWave();
    }

    void StartEndlessWave()
    {
        if (endlessWave == null)
        {
            Debug.LogError("Endless Wave is not assigned!");
            return;
        }

        changingWave = false;
        aliveEnemies.Clear();

        int totalEnemies = 0;

        foreach (var data in endlessWave.enemies)
            totalEnemies += data.amount;

        enemiesLeftToSpawn = totalEnemies;

        if (enemySlider != null)
        {
            enemySlider.minValue = 0;
            enemySlider.maxValue = totalEnemies;
            enemySlider.value = totalEnemies;
        }

        StartCoroutine(SpawnWaveRoutine(endlessWave));
    }

    IEnumerator StartGame()
    {
        DialogueActive = true;
        if (AudioManager.instance != null) AudioManager.instance.playDialogueBGM();

        if (dialogueRunner != null && !string.IsNullOrEmpty(introDialogueNode))
        {
            dialogueRunner.StartDialogue(introDialogueNode);
            dialogueCharacter.ShowCharacter();
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
            dialogueCharacter.HideCharacter();
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

        // Hitung total semua musuh
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

        StartCoroutine(SpawnWaveRoutine(wave));
    }

    IEnumerator SpawnWaveRoutine(WaveSO wave)
    {
        isSpawningWave = true;

        // 1. Kumpulkan semua musuh dari berbagai Element ke dalam satu "Kantong"
        List<GameObject> enemiesToSpawn = new List<GameObject>();
        
        foreach (EnemySpawnData data in wave.enemies)
        {
            for (int i = 0; i < data.amount; i++)
            {
                if (data.enemyPrefab != null)
                {
                    enemiesToSpawn.Add(data.enemyPrefab);
                }
            }
        }

        // 2. Acak (Shuffle) isi kantong agar elemen muncul secara acak
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            GameObject temp = enemiesToSpawn[i];
            int randomIndex = Random.Range(i, enemiesToSpawn.Count);
            enemiesToSpawn[i] = enemiesToSpawn[randomIndex];
            enemiesToSpawn[randomIndex] = temp;
        }

        // 3. Spawn musuh satu per satu dari kantong yang diacak
        foreach (GameObject prefab in enemiesToSpawn)
        {
            if (PlayerHealth.GameOver) yield break;

            IEnemy enemy = enemySpawner.SpawnSingleEnemy(prefab);
            if (enemy != null)
            {
                aliveEnemies.Add(enemy);
            }
            
            enemiesLeftToSpawn--;
            
            // Jeda sebelum memunculkan musuh berikutnya (ubah di Inspector untuk mengatur seberapa "spammy")
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawningWave = false;
        CheckWaveCompletion();
    }

    public void EnemyDied(IEnemy enemy)
    {
        if (PlayerHealth.GameOver || enemy == null) return;
            
        if (aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Remove(enemy);
        }

        if (enemySlider != null)
            enemySlider.value = aliveEnemies.Count + enemiesLeftToSpawn;

        CheckWaveCompletion();
    }

    private void CheckWaveCompletion()
    {
        if (aliveEnemies.Count == 0 && !isSpawningWave && !changingWave)
        {
            changingWave = true;

            if (PlayerPrefs.GetInt("Endless", 0) == 1)
            {
                changingWave = false;
                StartEndlessWave();
                return;
            }

            if (currentWaveIndex >= waves.Length - 1)
            {
                Debug.Log("FINAL WAVE COMPLETE!");
                LoadEndingCutscene();
                return;
            }

            StartCoroutine(BeginUpgradeSelection());
        }
    }   

    private void LoadEndingCutscene()
    {
        LevelLoader.instance.LoadScene("EndCutscene");
    }

    IEnumerator BeginUpgradeSelection()
    {
        yield return new WaitForSeconds(delayBeforeUpgrades);

        WaveSO wave = waves[currentWaveIndex];

        if (dialogueRunner != null && !string.IsNullOrEmpty(wave.upgradeDialogueNode))
        {
            DialogueActive = true;
            dialogueCharacter.ShowCharacter();
            if (AudioManager.instance != null) AudioManager.instance.playDialogueBGM();

            dialogueRunner.StartDialogue(wave.upgradeDialogueNode);
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

            dialogueCharacter.HideCharacter();
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