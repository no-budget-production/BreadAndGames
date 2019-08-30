using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Slider voiceSlider;

    [SerializeField] private Slider musicSlider;

    [SerializeField] private Slider fxSlider;

    [SerializeField] private Slider qualitySlider;

    [SerializeField] private AudioMixer masterMixer;

    private const string stringFXVolume = "SFXVolume";
    private const string stringMusicVolume = "MusicVolume";
    private const string stringVoiceVolume = "AtmoVolume";
    private const string stringMusicVol = "MusicVolume";

    private MusicSet curSettings;

    private SaveManager saveManager;

    private const float volumeFactor = 20.0f;

    private void Awake()
    {
        saveManager = SaveManager.Instance;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SaveSettings();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        System.GC.Collect();
    }

    public void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        curSettings = saveManager.LoadMusicSettings();

        musicSlider.value = curSettings.MusicVolume;
        fxSlider.value = curSettings.SFXVolume;
        voiceSlider.value = curSettings.VoiceVolume;

        VoiceSlider();
        MusicSlider();
        FXSlider();
    }

    private void SaveSettings()
    {
        curSettings = new MusicSet();

        curSettings.MusicVolume = musicSlider.value;
        curSettings.SFXVolume = fxSlider.value;
        curSettings.VoiceVolume = voiceSlider.value;

        saveManager.SaveMusicSettings(curSettings);
    }

    public void QualitySliderFunc()
    {
        int qualityLevel = Mathf.RoundToInt(qualitySlider.value);

        QualitySettings.SetQualityLevel(qualityLevel, true);
    }

    public void VoiceSlider()
    {
        //voiceSource.volume = voiceSlider.value;
        SetVoiceVolume(voiceSlider.value);
    }

    public void MusicSlider()
    {
        //musicSource.volume = musicSlider.value;
        SetMusicVolume(musicSlider.value);
    }

    public void FXSlider()
    {
        //fxSource.volume = fxSlider.value;
        SetFXVolume(fxSlider.value);
    }

    public void SetFXVolume(float lvl)
    {
        masterMixer.SetFloat(stringFXVolume, Mathf.Log10(lvl) * volumeFactor);
    }

    public void SetMusicVolume(float lvl)
    {
        masterMixer.SetFloat(stringMusicVolume, Mathf.Log10(lvl) * volumeFactor);
    }

    public void SetVoiceVolume(float lvl)
    {
        masterMixer.SetFloat(stringVoiceVolume, Mathf.Log10(lvl) * volumeFactor);
    }

    public void ClearVolume()
    {
        masterMixer.ClearFloat(stringMusicVol);
    }
}