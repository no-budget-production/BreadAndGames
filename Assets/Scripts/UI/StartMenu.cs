using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject loadingScreen; 

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
        loadingScreen.SetActive(enabled);
        SceneManager.LoadScene(ChangetoStartScene);
    }

}
