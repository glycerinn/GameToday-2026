using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public GameObject GamePanel;
    public TextMeshProUGUI resultText;

    private bool isLoading = false;

    private AudioManager audioManager;
    private LevelLoader levelLoader;

    private void Awake()
    {
        GameObject loaderObject = GameObject.FindGameObjectWithTag("LevelLoader");

        if (loaderObject != null)
        {
            levelLoader = loaderObject.GetComponent<LevelLoader>();
        }
    }

    private void Start()
    {
        if (GamePanel != null)
            GamePanel.SetActive(false);
    }

    private AudioManager GetAudioManager()
    {
        audioManager = AudioManager.instance;

        if (audioManager == null)
        {
            Debug.LogError("GameOverPanel: AudioManager.instance is NULL!");
        }

        return audioManager;
    }

    public void ShowWin()
    {
        Debug.Log("SHOW WIN");

        AudioManager audio = GetAudioManager();

        if (audio != null)
        {
            audio.playGameOverBGM();
        }

        if (resultText != null)
            resultText.text = "YOU WIN!";

        if (GamePanel != null)
            GamePanel.SetActive(true);
    }

    public void ShowLose()
    {
        Debug.Log("SHOW LOSE");

        AudioManager audio = GetAudioManager();

        if (audio != null)
        {
            audio.playGameOverBGM();
        }

        if (resultText != null)
            resultText.text = "YOU LOSE!";

        if (GamePanel != null)
            GamePanel.SetActive(true);
    }

    public void backtoMenu()
    {
        if (isLoading)
            return;

        isLoading = true;

        AudioManager audio = GetAudioManager();

        if (audio != null)
            audio.playClickSFX();

        if (levelLoader != null)
        {
            levelLoader.LoadMainMenu();
        }
        else
        {
            Debug.LogError("GameOverPanel: LevelLoader is NULL!");
        }
    }

    public void playAgain()
    {
        Time.timeScale = 1f;
        if (isLoading)
            return;

        isLoading = true;

        AudioManager audio = GetAudioManager();

        if (audio != null)
            audio.playClickSFX();

        if (levelLoader != null)
        {
            levelLoader.RetryGame();
        }
        else
        {
            Debug.LogError("GameOverPanel: LevelLoader is NULL!");
        }
    }
}