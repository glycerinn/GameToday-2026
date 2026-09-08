using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator animator;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    public static LevelLoader instance;

    private bool isLoading;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadGame()
    {
        LoadScene("SampleScene");
    }

    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }

    public void RetryGame()
    {
        LoadScene("SampleScene");
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading)
            return;

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;

        Time.timeScale = 1f;

        if (animator != null)
        {
            animator.SetTrigger("FadeIn");
        }

        yield return new WaitForSecondsRealtime(fadeDuration);

        SceneManager.LoadScene(sceneName);

        yield return null;

        if (animator != null)
        {
            animator.SetTrigger("FadeOut");
        }

        yield return new WaitForSecondsRealtime(fadeDuration);

        isLoading = false;
    }
}