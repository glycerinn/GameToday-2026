using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource BGM;
    public AudioSource SFX;

    public AudioClip MainMenuBGM;
    public AudioClip GameBGM;
    public AudioClip GameOverBGM;
    public AudioClip DialogueBGM;
    public AudioClip clicksfx;
    public AudioClip hoversfx;
    public AudioClip shotgunsfx;
    public AudioClip mmsfx;
    public AudioClip lowsfx;
    public AudioClip hammersfx;
    public AudioClip enemyshootsfx;
    public AudioClip diesfx;
    public AudioClip fallsfx;
    public AudioClip swapsfx;
    public AudioClip grapplingsfx;
    public AudioClip grappleenemywallsfx;
    public AudioClip grappleshootsfx;
    
    public static AudioManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (BGM.clip == clip && BGM.isPlaying)
            return;

        BGM.clip = clip;
        BGM.Play();
    }

    public void StopBGM()
    {
        BGM.Stop();
    }

    public void playMainMenuBGM()
    {
        BGM.clip = MainMenuBGM;
        BGM.Play();
    }

    public void playGameBGM()
    {
        PlayBGM(GameBGM);
    }

    public void playDialogueBGM()
    {
        PlayBGM(DialogueBGM);
    }

    public void playGameOverBGM()
    {
        BGM.clip = GameOverBGM;
        BGM.Play();
    }

    public void playClickSFX()
    {
        SFX.PlayOneShot(clicksfx);
    }

    public void playHoverSFX()
    {
        SFX.PlayOneShot(hoversfx);
    }


    public void playShotGunSFX()
    {
        SFX.PlayOneShot(shotgunsfx);
    }

    public void playEnemyShootSFX()
    {
        SFX.PlayOneShot(enemyshootsfx);
    }

    public void playDieSFX()
    {
        SFX.PlayOneShot(diesfx);
    }

    public void playHammerSFX()
    {
        SFX.PlayOneShot(hammersfx);
    }

    public void playLowSFX()
    {
        SFX.PlayOneShot(lowsfx);
    }

    public void playSwapSFX()
    {
        SFX.PlayOneShot(swapsfx);
    }

    public void playMMSFX()
    {
        SFX.PlayOneShot(mmsfx);
    }

    public void playFallSFX()
    {
        SFX.PlayOneShot(fallsfx);
    }

    public void playGrapplingSFX()
    {
        SFX.PlayOneShot(grapplingsfx);
    }

    public void playGrappleWallSFX()
    {
        SFX.PlayOneShot(grappleenemywallsfx);
    }

    public void playGrappleShootSFX()
    {
        SFX.PlayOneShot(grappleshootsfx);
    }
}