using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    public UIDocument uIDocument;
    private Label title;
    private Button button;
    private Button settingsbutton;
    private Button quitbutton;
    // private AudioManager audioManager;
    // public SettingsManager settings;

    private void Awake()
    {
        // audioManager = AudioManager.instance;
        uIDocument = GetComponent<UIDocument>();

        button = uIDocument.rootVisualElement.Q("PlayButton") as Button;
        button.RegisterCallback<ClickEvent>(OnPlayGame);

        // settingsbutton = uIDocument.rootVisualElement.Q("SettingsButton") as Button;
        // settingsbutton.RegisterCallback<ClickEvent>(OnSettings);

        // quitbutton = uIDocument.rootVisualElement.Q("QuitButton") as Button;
        // quitbutton.RegisterCallback<ClickEvent>(OnQuit);
    }

    // private void Start()
    // {
    //     int random = Random.Range(0, 3);

    //     if (random == 0)
    //     {
    //         audioManager.playLakeA();
    //     }
    //     else if (random == 1)
    //     {
    //         audioManager.playMountainA();
    //     }
    //     else
    //     {
    //         audioManager.playVillageA();
    //     }
    // }

    private void OnPlayGame(ClickEvent evt)
    {
        Debug.Log("pressed");
        // audioManager.playClickSFX();
        // Time.timeScale = 1f;

        // if (FadeManager.Instance != null)
        // {
        //     FadeManager.Instance.LoadSceneWithFade("Cutscene");
        // }
        // else
        // {
            SceneManager.LoadScene("SampleScene");
        // }
    }

    private void OnSettings(ClickEvent evt)
    {
        Debug.Log("pressed");
        // audioManager.playClickSFX();
        Time.timeScale = 1f;

        // settings.SetUp();

        uIDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnQuit(ClickEvent evt)
    {
        Debug.Log("pressed");
        // audioManager.playClickSFX();

        Time.timeScale = 1f;

        Application.Quit();
    }

    private void OnDisable()
    {
        if (button != null) button.UnregisterCallback<ClickEvent>(OnPlayGame);
        if (settingsbutton != null) settingsbutton.UnregisterCallback<ClickEvent>(OnSettings);
        if (quitbutton != null) quitbutton.UnregisterCallback<ClickEvent>(OnQuit);
    }
}