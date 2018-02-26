using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAnimEvents : MonoBehaviour
{

    public AudioClip[] WalkClips;
    public float highPitch;
    public float lowPitch;

    public AudioSource _AudioSource;

    private int StepCounter;

    void Start()
    {
        StepCounter = 1;
    }

    public void WalkSound()
    {
        if(StepCounter % StepCounter == 0)
        {
            _AudioSource.clip = WalkClips[0];
            _AudioSource.Play();
        }
        else if (StepCounter % StepCounter != 0)
        {
            _AudioSource.clip = WalkClips[1];
            _AudioSource.Play();
        }
        
    }
}
