using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISound : MonoBehaviour
{
    public AudioSource source;
    public AudioClip click;

    
    public void PlaySound()
    {
        source.PlayOneShot(click);
    }
}
