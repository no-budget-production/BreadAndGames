using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private const string musicVolumeString = "MusicVolume";
    private const string sFXVolumeString = "SFXVolume";
    private const string voiceVolumeString = "VoiceVolume";

    private const string inputSettingsString = "InputSettings";
    private const string languageSettingsString = "LanguageSettings";
    private const string playerNumberSettingsString = "playerNumberSettings";

    public static SaveManager _instance
    {
        get
        {
            return Instance;
        }
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void SaveMusicSettings(MusicSet editedSettings)
    {
        PlayerPrefs.SetFloat(musicVolumeString, editedSettings.MusicVolume);
        PlayerPrefs.SetFloat(sFXVolumeString, editedSettings.SFXVolume);
        PlayerPrefs.SetFloat(voiceVolumeString, editedSettings.VoiceVolume);
    }

    public MusicSet LoadMusicSettings()
    {
        MusicSet curSettings = new MusicSet();

        if (PlayerPrefs.HasKey(musicVolumeString))
        {
            curSettings.MusicVolume = PlayerPrefs.GetFloat(musicVolumeString);
            curSettings.SFXVolume = PlayerPrefs.GetFloat(sFXVolumeString);
            curSettings.VoiceVolume = PlayerPrefs.GetFloat(voiceVolumeString);
        }

        return curSettings;
    }

    public void SaveGameSettings(GameSet editedSettings)
    {
        PlayerPrefs.SetInt(languageSettingsString, editedSettings.languageSettings);
        PlayerPrefs.SetInt(inputSettingsString, editedSettings.inputSettings);
        PlayerPrefs.SetInt(playerNumberSettingsString, editedSettings.playerNumberSettings);
    }

    public GameSet LoadGameSettings()
    {
        GameSet curSettings = new GameSet();

        if (PlayerPrefs.HasKey(inputSettingsString))
        {
            curSettings.languageSettings = PlayerPrefs.GetInt(languageSettingsString);
            curSettings.inputSettings = PlayerPrefs.GetInt(inputSettingsString);
            curSettings.playerNumberSettings = PlayerPrefs.GetInt(playerNumberSettingsString);
        }

        return curSettings;
    }
}

public class MusicSet
{
    public float MusicVolume = 1.0f;
    public float SFXVolume = 1.0f;
    public float VoiceVolume = 1.0f;
}

public class GameSet
{
    public int languageSettings = 0;
    public int inputSettings = 0;
    public int playerNumberSettings = 1;
}
