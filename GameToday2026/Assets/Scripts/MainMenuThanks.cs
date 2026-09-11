using UnityEngine;

public class MainMenuThanks : MonoBehaviour
{
    public GameObject thanksText;

    private void Start()
    {
        if (thanksText != null)
            thanksText.SetActive(PlayerPrefs.GetInt("GameCompleted", 0) == 1);
    }
}