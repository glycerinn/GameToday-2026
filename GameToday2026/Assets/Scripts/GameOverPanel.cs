using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public GameObject GamePanel;
    public TextMeshProUGUI resultText;
    private bool isLoading = false;

    private AudioManager audioManager;
    private LevelLoader levelLoader;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
    } 

    void Start()
    {
        GamePanel.SetActive(false);
    }

    public void ShowWin()
    {
        audioManager.playGameOverBGM();
        resultText.text = "YOU WIN!";
        GamePanel.SetActive(true);
    }

    public void ShowLose()
    {
        audioManager.playGameOverBGM();
        resultText.text = "YOU LOSE!";
        GamePanel.SetActive(true);
    }
    
    public void backtoMenu()
    {
        if (isLoading)
            return;

        isLoading = true;

        audioManager.playClickSFX();

        if (levelLoader != null)
        {
            levelLoader.LoadMainMenu();
        }
    }

    public void playAgain()
    {
        if (isLoading)
            return;

        isLoading = true;

        audioManager.playClickSFX();

        if (levelLoader != null)
        {
            levelLoader.RetryGame();
        }
    }
}