using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool isLoading = false;
    public GameObject SkipButton;

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

    void Start()
    {
        audioManager.StopBGM();
        SkipButton.SetActive(false);
        StartCoroutine(ShowSkipButton());
    }

    public IEnumerator ShowSkipButton()
    {
        yield return new WaitForSeconds(3f);
        SkipButton.SetActive(true);
    }

    public void SkipVideo()
    {
        if (isLoading)
            return;

        isLoading = true;
        videoPlayer.Stop();

        levelLoader.LoadGame();
    }

    public void SkipVideoTwo()
    {
        if (isLoading)
            return;

        isLoading = true;
        videoPlayer.Stop();

        levelLoader.LoadMainMenu();
    }
}