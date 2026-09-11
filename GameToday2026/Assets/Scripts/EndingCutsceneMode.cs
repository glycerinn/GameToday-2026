using UnityEngine;

public class EndingCutsceneMode : MonoBehaviour
{
    private void Awake()
    {
        PlayerPrefs.SetInt("Endless", 1);
        PlayerPrefs.Save();
    }
}