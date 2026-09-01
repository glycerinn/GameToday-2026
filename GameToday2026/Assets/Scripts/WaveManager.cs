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
    public UpgradeManager upgradeManager;
    public DialogueRunner dialogueRunner;
    public MapManager mapManager;

    [Header("Dialogue")]
    public string introDialogueNode = "GameIntro";
    public static bool DialogueActive { get; private set; }

    [Header("Waves")]
    public WaveSO[] waves;

    [Header("UI")]
    public TextMeshProUGUI nextWaveText;
    public Slider enemySlider;
    public Slider waveSlider;

    [Header("Timing")]
    public float delayBeforeUpgrades = 2f;
    public float nextWaveTextDuration = 1.5f;

    private int currentWaveIndex;
    private int currentWaveEnemyCount;
    private List<IEnemy> aliveEnemies = new List<IEnemy>();
    private bool changingWave;

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

        if (dialogueRunner != null && !string.IsNullOrEmpty(introDialogueNode))
        {
            dialogueRunner.StartDialogue(introDialogueNode);
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
        }

        DialogueActive = false;

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

        Debug.Log("STARTING WAVE " + wave.waveNumber);

        aliveEnemies.Clear();

        List<IEnemy> spawnedEnemies = enemySpawner.SpawnEnemies(wave.enemies);

        foreach (IEnemy enemy in spawnedEnemies)
        {
            if (enemy != null)
                aliveEnemies.Add(enemy);
        }

        currentWaveEnemyCount = aliveEnemies.Count;

        if (enemySlider != null)
        {
            enemySlider.minValue = 0;
            enemySlider.maxValue = currentWaveEnemyCount;
            enemySlider.value = currentWaveEnemyCount;
        }

        if (waveSlider != null)
            waveSlider.value = currentWaveIndex;

        Debug.Log("Tracked enemies: " + aliveEnemies.Count);
    }

    public void EnemyDied(IEnemy enemy)
    {
        if (!aliveEnemies.Contains(enemy))
        {
            Debug.LogWarning("Enemy died but was not being tracked by WaveManager.");
            return;
        }

        aliveEnemies.Remove(enemy);

        if (enemySlider != null)
            enemySlider.value = aliveEnemies.Count;

        Debug.Log("Enemy died. Remaining enemies: " + aliveEnemies.Count);

        if (aliveEnemies.Count == 0 && !changingWave)
        {
            changingWave = true;

            Debug.Log("ALL ENEMIES IN WAVE " + (currentWaveIndex + 1) + " ARE DEAD!");

            if (currentWaveIndex >= waves.Length - 1)
            {
                Debug.Log("FINAL WAVE COMPLETE!");

                if (nextWaveText != null)
                {
                    nextWaveText.text = "ALL WAVES COMPLETE!";
                    nextWaveText.gameObject.SetActive(true);
                }

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

            dialogueRunner.StartDialogue(wave.upgradeDialogueNode);
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);

            DialogueActive = false;
        }

        if (upgradeManager != null)
        {
            upgradeManager.ShowUpgradeChoices();
        }
        else
        {
            Debug.LogError("UpgradeManager is not assigned!");
        }
    }

    public void UpgradeSelected()
    {
        Debug.Log("UPGRADE CONFIRMED - CHANGING MAP");

        if (mapManager != null)
        {
            mapManager.SetRandomMap();
        }

        StartCoroutine(ContinueToNextWave());
    }

    IEnumerator ContinueToNextWave()
    {
        currentWaveIndex++;

        if (waveSlider != null)
            waveSlider.value = currentWaveIndex;

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
            nextWaveText.text = "NEXT WAVE " + nextWave.waveNumber;
            nextWaveText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(nextWaveTextDuration);

        if (nextWaveText != null)
            nextWaveText.gameObject.SetActive(false);

        StartWave();
    }
}