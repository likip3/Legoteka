using UnityEngine;
using UnityEngine.UI;

public class ChangeAudioClip : MonoBehaviour
{
    public Button button;
    public AudioSource audioSource;
    public string audioClip;

    private void Start()
    {
        button.onClick.AddListener(ChangeClip);
    }

    private void ChangeClip()
    {
        StartCoroutine(LoadAudioClipAsync());
    }

    private System.Collections.IEnumerator LoadAudioClipAsync()
    {
        ResourceRequest request = Resources.LoadAsync<AudioClip>("music/" + audioClip);

        yield return request;

        if (audioSource.isPlaying && audioSource.clip.ToString().Split(' ')[0] == audioClip)
        {
            audioSource.Stop();
        }

        else if (request.asset is AudioClip loadedClip)
        {
            audioSource.clip = loadedClip;
            audioSource.Play();
        }
    }
}