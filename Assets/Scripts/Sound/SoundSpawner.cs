using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSpawner : MonoBehaviour
{
    public AudioClip[] AudioClipsArray;
    public float highPitch;
    public float lowPitch;

    public AudioSource _AudioSource;

    private bool PlayOnce;

    void Start()
    {
        PlayOnce = true;
    }

    void Update()
    {
        if (PlayOnce)
        {

            SoundRandomizer.RandomizeSfx(lowPitch, highPitch, _AudioSource, AudioClipsArray);
            PlayOnce = false;
        }
    }


}
