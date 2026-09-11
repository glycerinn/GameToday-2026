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

    private int currentLine;
    private Coroutine typingCoroutine;

    private bool isTyping;
    private bool isTransitioning;

    private string currentText;

    private TextMeshProUGUI currentTextObject;

    private LevelLoader levelLoader;

    private void Awake()
    {
        levelLoader = LevelLoader.instance;

        if (levelLoader == null)
        {
            GameObject loaderObject =
                GameObject.FindGameObjectWithTag("LevelLoader");

            if (loaderObject != null)
                levelLoader = loaderObject.GetComponent<LevelLoader>();
        }
    }

    private void Start()
    {
        ShowDialogue();
    }

    private void Update()
    {
        if (isTransitioning)
            return;

        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    private void HandleClick()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            currentTextObject.text = currentText;
            isTyping = false;
            return;
        }

        currentLine++;

        if (currentLine < dialogues.Length)
        {
            ShowDialogue();
        }
        else
        {
            isTransitioning = true;

            if (levelLoader != null)
                levelLoader.LoadGame();
        }
    }

    private void ShowDialogue()
    {
        currentText = dialogues[currentLine];

        GameObject newLine =
            Instantiate(dialogueLinePrefab, dialogueContainer);

        currentTextObject =
            newLine.GetComponent<TextMeshProUGUI>();

        typingCoroutine =
            StartCoroutine(TypeDialogue(currentText));
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