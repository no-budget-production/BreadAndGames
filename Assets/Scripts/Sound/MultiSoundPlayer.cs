using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioClipClassArrayArray
{
    public AudioClip[] AudioClips;
}

public class MultiSoundPlayer : MonoBehaviour
{
    // SetUp 

    public AudioSource VoiceSource;
    public AudioClipClassArrayArray[] AudioClipsArray;

    private int _currentVoiceInt;
    private int _currentSoundInt;

    private int _voicesInt;
    private int[] _soundsInt;
    public int[] _lastSound;

    // Game Logic

    private int TakingDamage;
    private int Empty;

    public void Awake()
    {
        _voicesInt = AudioClipsArray.Length;

        _soundsInt = new int[_voicesInt];

        _lastSound = new int[_voicesInt];

        for (int i = 0; i < _voicesInt; i++)
        {
            _soundsInt[i] = AudioClipsArray[i].AudioClips.Length;
        }

        TakingDamage = Random.Range(0, _soundsInt[0]);
        Empty = Random.Range(0, _soundsInt[1]);
    }

    public void Stop()
    {
        VoiceSource.Stop();
    }

    public void PlaySound()
    {
        CycleSoundInList();
        VoiceSource.clip = AudioClipsArray[_currentVoiceInt].AudioClips[_currentSoundInt];
        VoiceSource.Play();
    }

    public void PlayRandomSound()
    {
        //for (int i = 0; i < _voicesInt; i++)
        //{
        //    int a = i;
        //    for (int j = 0; j < AudioClipsArray[a].AudioClips.Length; j++)
        //    {
        //        Debug.Log("List " + a + "Sound " + j + "Var " + AudioClipsArray[a].AudioClips[j].name);
        //    }
        //}
        Stop();

        PlaySound();
        if (AudioClipsArray[_currentVoiceInt].AudioClips.Length - 1 <= _currentSoundInt)
        {
            _currentVoiceInt++;
            _currentSoundInt = 0;
        }
        else
        {
            _currentSoundInt++;
        }
    }

    private void CycleSoundInList()
    {
        _currentSoundInt = _lastSound[_currentVoiceInt];

        if (_lastSound[_currentVoiceInt] >= _soundsInt[_currentVoiceInt] - 1)
        {
            _lastSound[_currentVoiceInt] = 0;
        }
        else
        {
            _lastSound[_currentVoiceInt]++;
        }
        _currentSoundInt = _lastSound[_currentVoiceInt];
    }

    public float GetClipLenght()
    {
        return VoiceSource.clip.length;
    }

    public void TakingDamageSound()
    {
        Stop();
        _currentVoiceInt = 0;
        PlaySound();
    }

    public void EmptySound()
    {
        Stop();
        _currentVoiceInt = 1;
        PlaySound();
    }
}