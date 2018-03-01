using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIScript : MonoBehaviour
{
    public GameObject MainMenu;

    public GameObject HelpMenu;

    //public CheatHelp CheatHelp;

    private bool escapeButton;
    private bool escapeButtonXbox;

    private bool _isPause = false;

    private bool _aboutToPause = false;
    private bool _aboutToUnpause = false;

    private int sceneindex;

    public Text GameOverText;

    public Button ResumeButton;

    public bool isGameOver;

    public AudioSource Announcer;

    //AudioSetUp
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    //public AudioMixerSnapshot paused;
    //public AudioMixerSnapshot unpaused;

    //public AudioMixer masterMixer;

    /// Voice

    //public float curLaughingCD;
    //public float LaughingCD;

    private void Awake() // Moved from Update to Awake function
    {
        sceneindex = SceneManager.GetActiveScene().buildIndex;
        escapeButton = Input.GetButtonDown("Cancel"); //from Escape to Cancel
        escapeButtonXbox = Input.GetButtonDown("CancelXbox"); //from Escape to Cancel
        GameOverText.text = "";
    }

    private void Update()
    {
        PauseMenu();
        RestartGame();
        QuitGame();

        if (isGameOver)
        {
            DamageImage.color = Color.Lerp(DamageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        MainMenu.SetActive(false);
        Cursor.visible = false;
        _isPause = false;

    }

    //GameOver
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    public float GameOverDelay;
    public Image DamageImage;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    public void GameOver()
    {
        isGameOver = true;
        DamageImage.color = flashColour;
        StopCoroutine(GameOverMenu());
        StartCoroutine(GameOverMenu());
    }

    public virtual IEnumerator GameOverMenu()
    {
        yield return new WaitForSeconds(GameOverDelay);

        TimeScaleOff();
    }

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
        Cursor.visible = false;

        //PlaySound(0);
    }

    public void RestartButton()
    {
        GameManager.Instance.transform.parent = GameManager.Instance.InstanceRef.transform;

        Destroy(GameManager.Instance.InstanceRef.gameObject);

        MainMenu.SetActive(false);

        GameOverText.text = "";
        AreYouSure_Restart();
        Cursor.visible = false;
        //PlaySound(1);
    }

    public void AreYouSure_Restart()
    {
        //SceneManager.UnloadSceneAsync(sceneindex);

        SceneManager.LoadScene(sceneindex, LoadSceneMode.Single);
        Cursor.visible = false;
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

    public void TimeScaleOn()
    {
        //unpaused.TransitionTo(0.5f);

        Announcer.UnPause();

        Time.timeScale = 1;
        MainMenu.SetActive(false);
        _isPause = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _aboutToUnpause = false; // Added
    }

    public void TimeScaleOff()
    {
        int areBothPlayersDead = 0;
        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            if (GameManager.Instance.Players[i].isDeadTrigger == true)
            {
                areBothPlayersDead++;
            }
        }

        if (areBothPlayersDead == GameManager.Instance.Players.Count)
        {
            ResumeButton.interactable = false;
        }

        Announcer.Pause();

        Time.timeScale = 0f;
        MainMenu.SetActive(true);
        _isPause = true;

        //CheatHelp.UpdatePlayerButtons();

        //Cursor.visible = true;
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



