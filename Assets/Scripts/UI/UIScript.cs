using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIScript : MonoBehaviour {

    private void Awake() // Moved from Update to Awake function
    {
        sceneindex = SceneManager.GetActiveScene().buildIndex;
        escapeButton = Input.GetButtonDown("Cancel"); //from Escape to Cancel
        escapeButtonXbox = Input.GetButtonDown("CancelXbox"); //from Escape to Cancel
    }

    private void Update()
    {
        PauseMenu();
        RestartGame();
        QuitGame();
    }

    private void Start()
    {
        Time.timeScale = 1;
        MainMenu.SetActive(false);
        _isPause = false;
    }

    //Bools&Stuff
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    private bool _isPause = false;

    private bool _aboutToPause = false;
    private bool _aboutToUnpause = false;

    private int sceneindex;

    /// /////////////////////////////////////////////////////////////////////////////////////////////////////


    //GameObjects
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    public GameObject MainMenu;

    //public AudioClip[] AudioClips;     //Added
    //public AudioSource AudioSource;     //Added

    //public Slider voiceSlider;
    //public AudioSource voiceSource;

    //public Slider musicSlider;
    //public AudioSource musicSource;

    //public Slider fxSlider;
    //public AudioSource fxSource;


    /// /////////////////////////////////////////////////////////////////////////////////////////////////////


    //BoolButtons
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    private bool escapeButton;
    private bool escapeButtonXbox;

    /// /////////////////////////////////////////////////////////////////////////////////////////////////////


    //AudioSetUp
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    //public AudioMixerSnapshot paused;
    //public AudioMixerSnapshot unpaused;

    //public AudioMixer masterMixer;

    /// Voice

    //public float curLaughingCD;
    //public float LaughingCD;

    /// /////////////////////////////////////////////////////////////////////////////////////////////////////


    //ClickableUIButtons
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ChangeToScene_Controller(int ChangetoStartScene)
    {
        SceneManager.LoadScene(ChangetoStartScene);
    }

    public void BackButton()
    {

        //Time.timeScale = 1;
        MainMenu.SetActive(false);

        //PlaySound(0);
    }

    public void RestartButton()
    {
        MainMenu.SetActive(false);

        AreYouSure_Restart();

        //PlaySound(1);
    }

    public void AreYouSure_Restart()
    {
        SceneManager.UnloadSceneAsync(sceneindex);
        SceneManager.LoadScene(sceneindex, LoadSceneMode.Single);

        //PlaySound(1);
    }

    public void ExitButton()
    {
        MainMenu.SetActive(false);

        AreYouSure_Exit();

        TimeScaleOn();

        //PlaySound(1);
    }

    public void AreYouSure_Exit()
    {
        Application.Quit();

        //PlaySound(1);
    }

    //public void VoiceSlider()
    //{
    //    voiceSource.volume = voiceSlider.value;

    //}

    //public void VoiceSliderHandle()
    //{
    //    if (curLaughingCD <= Time.time)
    //    {
    //        VoiceOrigion.Laughing();
    //        curLaughingCD = Time.time + LaughingCD + VoiceOrigion.GetClipLenght();
    //    }
    //}

    //public void MusicSlider()
    //{
    //    musicSource.volume = musicSlider.value;
    //}

    //public void FXSlider()
    //{
    //    fxSource.volume = fxSlider.value;
    //}


    /// /////////////////////////////////////////////////////////////////////////////////////////////////////


    //InputButtonFunctions
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    void PauseMenu()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("CancelXbox"))
        {
            TimeScaleToggle1();
        }
    }

    void RestartGame()
    {
        if (_isPause == true)
        {
            if (Input.GetButtonDown("Restart") || Input.GetButtonDown("RestartXbox"))
            {
                RestartButton();
            }
        }
    }

    void QuitGame()
    {
        if (_isPause == true)
        {
            if (Input.GetButtonDown("Exit") || Input.GetButtonDown("ExitXbox"))
            {
                ExitButton();
            }
        }
    }

    /// /////////////////////////////////////////////////////////////////////////////////////////////////////


    //TimeScale Functions
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void TimeScaleToggle1() // Cleaner - since BackButton used TimeSacleOn too
    {
        if (!_isPause)
        {
            TimeScaleToggle();
        }
        else
        {
            TimeScaleToggle();
        }
    }


    /// /////////////////////////////////////////////////////////////////////////////////////////////////////


    //TimeScale Functions
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////


    private void TimeScaleToggle() // Cleaner - since BackButton used TimeSacleOn too
    {

        if (!_isPause)
        {
            TimeScaleOff();

            //PlaySound(1);
        }
        else
        {
            TimeScaleOn();

            //PlaySound(0);
        }

    }

    private void TimeScaleOn()
    {
        //unpaused.TransitionTo(0.5f);

        Time.timeScale = 1;
        MainMenu.SetActive(false);
        _isPause = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _aboutToUnpause = false; // Added
    }

    private void TimeScaleOff()
    {
        Time.timeScale = 0f;
        MainMenu.SetActive(true);
        _isPause = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _aboutToPause = false; // Added

    }

    /// /////////////////////////////////////////////////////////////////////////////////////////////////////


    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    //Sound Functions
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    //public void SetFXVolume(float lvl)
    //{
    //    masterMixer.SetFloat("FXVolume", lvl);
    //}

    //public void SetMusicVolume(float lvl)
    //{
    //    masterMixer.SetFloat("MusicVolume", lvl);
    //}

    //public void SetVoiceVolume(float lvl)
    //{
    //    masterMixer.SetFloat("VoiceVolume", lvl);
    //}

    //public void ClearVolume()
    //{
    //    masterMixer.ClearFloat("musicVol");
    //}

    ///// UI Button Sounds

    //public void PlaySound(int i)
    //{
    //    AudioSource.clip = AudioClips[i];
    //    AudioSource.Play();
    //}

    /// /////////////////////////////////////////////////////////////////////////////////////////////////////
}



