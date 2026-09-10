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

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager.StopBGM();
        SkipButton.SetActive(false);
        StartCoroutine(ShowSkipButton());
        videoPlayer.loopPointReached += LoadNextScene;
        videoPlayer.Play();
    }

    public void LoadNextScene(VideoPlayer videoPlayer)
    {
        if (isLoading)
            return;

        isLoading = true;
        // StartCoroutine(LoadNextLevel());
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
        // StartCoroutine(LoadNextLevel());
    }

    void OnDestroy()
    {
        videoPlayer.loopPointReached -= LoadNextScene;
    }

    // IEnumerator LoadNextLevel()
    // {
    //     videoPlayer.Stop();
    //     yield return StartCoroutine(Transition.Instance.PlayTransition());
    //     SceneManager.LoadScene("SampleScene");
    //     yield return StartCoroutine(Transition.Instance.EndTransition());
    // }
}