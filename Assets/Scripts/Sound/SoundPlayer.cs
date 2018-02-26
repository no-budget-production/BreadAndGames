using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip[] AudioClipsArray;

    private int _currentSoundInt;
    private int _lastSound;

    public void Play()
    {
        //AudioSource.Stop();
        AudioSource.clip = AudioClipsArray[0];
        AudioSource.Play();
    }

    public void PlayCycle()
    {
        CycleSoundInList();
        AudioSource.clip = AudioClipsArray[_currentSoundInt];
        AudioSource.Play();
    }

    public void Stop()
    {
        AudioSource.Stop();
    }

    private void CycleSoundInList()
    {
        if (_lastSound >= AudioClipsArray.Length - 1)
        {
            _lastSound = 0;
        }
        else
        {
            _lastSound++;
        }
        _currentSoundInt = _lastSound;
    }

    public float GetClipLenght()
    {
        return AudioSource.clip.length;
    }
}
