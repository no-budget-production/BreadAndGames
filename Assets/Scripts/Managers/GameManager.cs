using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance = null;

    public PrefabLoader prefabLoader;

    void Awake()
    {
        InitGame();
    }

    public void GameOver()
    {
        enabled = false;
    }

    void InitGame()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        prefabLoader.LoadPrefabs();
    }
}
