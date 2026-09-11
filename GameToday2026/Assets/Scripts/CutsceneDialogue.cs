using System.Collections;
using TMPro;
using UnityEngine;

public class CutsceneDialogue : MonoBehaviour
{
    public Transform dialogueContainer;
    public GameObject dialogueLinePrefab;

    [TextArea]
    public string[] dialogues;

    public float typingSpeed = 0.05f;

    [Header("Scene")]
    public string gameSceneName = "SideScroller";

    private int currentLine;
    private Coroutine typingCoroutine;

    private bool isTyping;
    private bool isTransitioning; // NEW

    private string currentText;

    private TextMeshProUGUI currentTextObject;

    private AudioManager audioManager;
    private LevelLoader levelLoader;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        GameObject loaderObject = GameObject.FindGameObjectWithTag("LevelLoader");

        if (loaderObject != null)
        {
            levelLoader = loaderObject.GetComponent<LevelLoader>();
        }
    }

    private void Start()
    {
        ShowDialogue();
    }

    private void Update()
    {
        // Ignore clicks once scene transition has started
        if (isTransitioning)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        // Finish current text first
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);

            currentTextObject.text = currentText;
            isTyping = false;

            return;
        }

        // Move to next dialogue
        currentLine++;

        // More dialogue remaining
        if (currentLine < dialogues.Length)
        {
            ShowDialogue();
        }
        // All dialogue finished
        else
        {
            // LOCK INPUT IMMEDIATELY
            isTransitioning = true;

            Debug.Log("Cutscene finished. Loading game scene...");
            levelLoader.LoadGame();
        }
    }

    private void ShowDialogue()
    {
        currentText = dialogues[currentLine];

        GameObject newLine = Instantiate(
            dialogueLinePrefab,
            dialogueContainer
        );

        currentTextObject =
            newLine.GetComponent<TextMeshProUGUI>();

        typingCoroutine = StartCoroutine(
            TypeDialogue(currentText)
        );
    }

    private IEnumerator TypeDialogue(string text)
    {
        isTyping = true;

        currentTextObject.text = "";

        foreach (char letter in text)
        {
            currentTextObject.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}