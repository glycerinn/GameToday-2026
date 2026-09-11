using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    public UIDocument uIDocument;
    private Label title;
    private Button button;
    private Button creditsbutton;
    private Button quitbutton;
    private AudioManager audioManager;
    private LevelLoader levelLoader;

    public CreditsManager credits;

    private void Awake()
    {
        Time.timeScale=1f;
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        uIDocument = GetComponent<UIDocument>();

        button = uIDocument.rootVisualElement.Q("PlayButton") as Button;
        button.RegisterCallback<ClickEvent>(OnPlayGame);

        creditsbutton = uIDocument.rootVisualElement.Q("CreditsButton") as Button;
        creditsbutton.RegisterCallback<ClickEvent>(OnCredits);

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

        levelLoader.StartStory();
    }

    private void OnCredits(ClickEvent evt)
    {
        Debug.Log("pressed");
        audioManager.playClickSFX();
        Time.timeScale = 1f;

        credits.CreditsSetUp();

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
        if (creditsbutton != null) creditsbutton.UnregisterCallback<ClickEvent>(OnCredits);
        if (quitbutton != null) quitbutton.UnregisterCallback<ClickEvent>(OnQuit);
    }
}