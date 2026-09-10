using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    public MainMenuEvents mainMenuEvents;
    public void CreditsSetUp()
    {
        gameObject.SetActive(true);
    }

    public void CreditsLoadMenu()
    {
        if (mainMenuEvents != null && mainMenuEvents.uIDocument != null)
        {
            mainMenuEvents.uIDocument.rootVisualElement.style.display = UnityEngine.UIElements.DisplayStyle.Flex;
        }
        gameObject.SetActive(false);
    }
}