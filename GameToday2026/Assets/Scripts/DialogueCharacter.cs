using UnityEngine;

public class DialogueCharacter : MonoBehaviour
{
    public GameObject characterSprite;

    public void ShowCharacter()
    {
        characterSprite.SetActive(true);
    }

    public void HideCharacter()
    {
        characterSprite.SetActive(false);
    }
}