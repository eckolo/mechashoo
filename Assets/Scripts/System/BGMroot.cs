using UnityEngine;
using System.Collections;

public class BGMroot : Methods
{
    AudioSource _audioSource = null;
    public AudioSource audioSource
    {
        get
        {
            if(_audioSource != null) return _audioSource;
            return _audioSource = GetComponent<AudioSource>();
        }
    }

    public void Update()
    {
        audioSource.volume = Volume.bgm * Volume.BASE_BGM;
    }
}
