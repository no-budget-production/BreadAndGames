using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwoLevelScript : MonoBehaviour
{
    public AudioClip alarm;

    public AudioSource _AudioSource1; 

    public void PlayAlarm()
    {
        _AudioSource1.clip = alarm;
        _AudioSource1.Play();
    }

}
