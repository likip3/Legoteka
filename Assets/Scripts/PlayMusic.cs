using UnityEngine;
using UnityEngine.UI;

public class PlayMusic : MonoBehaviour
{
    public AudioClip audioClip;
    public Button playButton;
    public AudioSource audioSource;

    void Start()
    {
        playButton.onClick.AddListener(PlayAudio);
    }

    void PlayAudio()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}