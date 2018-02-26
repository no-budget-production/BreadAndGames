using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAnimEvents : MonoBehaviour
{
    public AudioClip[] WalkClips;
    public float highPitch;
    public float lowPitch;

    public AudioSource _AudioSource;

    public void WalkSound()
    {
        SoundRandomizer.RandomizeSfx(lowPitch, highPitch, _AudioSource, WalkClips);
    }

}
