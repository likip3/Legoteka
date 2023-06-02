using UnityEngine;
using UnityEngine.UI;

public class PlayMusic : MonoBehaviour
{
    public AudioClip[] audioClips;
    public Button[] playButtons;
    private Image[] buttonImages;
    public Text[] buttonTexts;
    public Sprite initialSprite;
    public Sprite selectedSprite;
    public AudioSource audioSource;

    void Start()
    {
        buttonImages = new Image[playButtons.Length];

        for (int i = 0; i < playButtons.Length; i++)
        {
            int index = i;
            playButtons[i].onClick.AddListener(() => PlayAudio(index));
            buttonImages[i] = playButtons[i].GetComponent<Image>();
        }

        CheckPlayingMusic();
    }

    void CheckPlayingMusic()
    {
        if (audioSource.isPlaying)
        {
            for (int i = 0; i < audioClips.Length; i++)
            {
                if (audioSource.clip == audioClips[i])
                {
                    buttonImages[i].sprite = selectedSprite;
                    buttonTexts[i].color = Color.black;
                    break;
                }
            }
        }
    }

    void PlayAudio(int index)
    {
        if (audioSource.isPlaying && audioSource.clip == audioClips[index])
        {
            audioSource.Stop();
            ResetButtonStates();
        }
        else if (index >= 0 && index < audioClips.Length)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
            ResetButtonStates();
            buttonImages[index].sprite = selectedSprite;
            buttonTexts[index].color = Color.black;
        }
    }

    void ResetButtonStates()
    {
        for (int i = 0; i < buttonImages.Length; i++)
        {
            buttonImages[i].sprite = initialSprite;
            buttonTexts[i].color = Color.white;
        }
    }
}