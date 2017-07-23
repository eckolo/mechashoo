using UnityEngine;
using System.Collections;

public class BGMroot : Methods
{
    AudioSource _audioSource = null;
    public float baseVolume { get; set; } = 1;

    public AudioSource audioSource
    {
        get {
            if(_audioSource != null) return _audioSource;
            return _audioSource = GetComponent<AudioSource>();
        }
    }

    public override void Update()
    {
        base.Update();
        audioSource.volume = baseVolume * Configs.Volume.bgm * Configs.Volume.BASE_BGM;
    }
}
