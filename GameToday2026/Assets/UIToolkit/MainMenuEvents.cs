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
    private AudioManager audioManager;
    private LevelLoader levelLoader;

    // public SettingsManager settings;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        uIDocument = GetComponent<UIDocument>();

        button = uIDocument.rootVisualElement.Q("PlayButton") as Button;
        button.RegisterCallback<ClickEvent>(OnPlayGame);

        // settingsbutton = uIDocument.rootVisualElement.Q("SettingsButton") as Button;
        // settingsbutton.RegisterCallback<ClickEvent>(OnSettings);

        quitbutton = uIDocument.rootVisualElement.Q("QuitButton") as Button;
        quitbutton.RegisterCallback<ClickEvent>(OnQuit);
    }

    private void Start()
    {
        audioManager.playMainMenuBGM();
    }

    private void OnPlayGame(ClickEvent evt)
    {
        Debug.Log("pressed");
        audioManager.playClickSFX();
        Time.timeScale = 1f;

        levelLoader.LoadGame();
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