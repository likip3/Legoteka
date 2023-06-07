using UnityEngine;
using UnityEngine.UI;

public class Music : MonoBehaviour
{
    public Image[] buttonImages;
    private Text[] buttonTexts;
    public Sprite initialSprite;
    public Sprite selectedSprite;
    public AudioSource audioSource;

    void Start()
    {
        buttonTexts = new Text[buttonImages.Length];
        for (int i = 0; i < buttonImages.Length; i++)
        {
            buttonTexts[i] = buttonImages[i].GetComponentInChildren<Text>();
        }
    }

    void Update()
    {
        if (audioSource.isPlaying)
            {
                for (int i = 0; i < buttonImages.Length; i++)
                {
                    if (audioSource.clip.ToString().Split(' ')[0] == i.ToString())
                    {
                        ResetButtonStates();
                        buttonImages[i - 1].sprite = selectedSprite;
                        buttonTexts[i - 1].color = Color.black;
                        break;
                    }
                }
            }

        else ResetButtonStates();
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