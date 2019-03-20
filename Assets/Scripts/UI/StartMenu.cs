using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
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
        SceneManager.LoadScene(ChangetoStartScene);
    }

}
