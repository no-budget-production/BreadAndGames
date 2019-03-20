using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
#endif

public class UIScript : MonoBehaviour
{
    public GameObject MainMenu;

    public GameObject HelpMenu;

    public GameObject StatsMenu;

    //public CheatHelp CheatHelp;

    public StatsScreen StatsScreen;

    private bool escapeButton;
    private bool escapeButtonXbox;

    private bool _isPause = false;

    private bool _aboutToPause = false;
    private bool _aboutToUnpause = false;

    private int sceneindex;

    public int CreditsBuildIndex;

    public Text GameOverText;

    public Button ResumeButton;

    public bool isGameOver;

    public AudioSource Announcer;

    private StatsTracker statsTracker;

    //AudioSetUp
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    //public AudioMixerSnapshot paused;
    //public AudioMixerSnapshot unpaused;

    //public AudioMixer masterMixer;

    /// Voice

    //public float curLaughingCD;
    //public float LaughingCD;

    private GameManager gameManager;

    private void Awake() // Moved from Update to Awake function
    {
        CreditsBuildIndex = SceneManager.sceneCountInBuildSettings - 1;


        escapeButton = Input.GetButtonDown("Cancel"); //from Escape to Cancel
        escapeButtonXbox = Input.GetButtonDown("CancelXbox"); //from Escape to Cancel
        GameOverText.text = "";

        gameManager = GameManager.Instance;
        statsTracker = StatsTracker.Instance;
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
        sceneindex = SceneManager.GetActiveScene().buildIndex;

        Time.timeScale = 1;
        MainMenu.SetActive(false);
        StatsMenu.SetActive(false);
        Cursor.visible = false;
        _isPause = false;


    }

    public void UpDateTime()
    {


    }

    //GameOver
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////

    public float GameOverDelay;
    public Image DamageImage;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

    public void FinalStats()
    {
        StatsScreen.UpdateStats();
        TimeScaleOff();
    }

    public void GameOver()
    {
        StatsScreen.UpdateStats();
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
        //if (GameManager.Instance != null)
        //{
        //    if (GameManager.Instance.InstanceRef != null)
        //    {
        //        GameManager.Instance.transform.parent = GameManager.Instance.InstanceRef.transform;

        //        Destroy(GameManager.Instance.InstanceRef.gameObject);
        //    }
        //}

        statsTracker.ResetStats();

        MainMenu.SetActive(false);

        GameOverText.text = "";
        AreYouSure_Restart();
        Cursor.visible = false;
        //PlaySound(1);
    }

    public void AreYouSure_Restart()
    {
        //SceneManager.UnloadSceneAsync(sceneindex);

        if (!(CreditsBuildIndex == sceneindex))
        {
            SceneManager.LoadScene(sceneindex, LoadSceneMode.Single);
        }
        else
        {
            //SceneManager.UnloadSceneAsync(sceneindex);
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

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

        if (Announcer != null)
        {
            Announcer.UnPause();
        }

        Time.timeScale = 1;
        MainMenu.SetActive(false);
        _isPause = false;

        StatsMenu.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _aboutToUnpause = false; // Added
    }

    public void TimeScaleOff()
    {
        StatsMenu.SetActive(true);
        StatsScreen.UpdateStats();

        int areBothPlayersDead = 0;
        for (int i = 0; i < gameManager.Players.Count; i++)
        {
            if (gameManager.Players[i].isDeadTrigger == true)
            {
                areBothPlayersDead++;
            }
        }

        if (areBothPlayersDead == gameManager.Players.Count)
        {
            if (ResumeButton != null)
            {
                ResumeButton.interactable = false;
            }
        }

        if (Announcer != null)
        {
            Announcer.Pause();
        }

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



