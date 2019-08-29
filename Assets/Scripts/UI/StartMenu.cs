using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject loadingScreen;

    [SerializeField] GameObject pressAScreen;

    void Update()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("CancelXbox"))
        {
            Application.Quit();
        }

        if (Input.GetButtonDown("Submit"))
        {
            ChangeToScene_Controller(1);
        }
    }

    public void ChangeToScene_Controller(int ChangetoStartScene)
    {
        pressAScreen.SetActive(false);
        loadingScreen.SetActive(enabled);
        SceneManager.LoadScene(ChangetoStartScene);
    }

}
