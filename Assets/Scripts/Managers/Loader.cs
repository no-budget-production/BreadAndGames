using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameManager gameManager;
    public InstanceRef InstanceRef;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager curGameManager = Instantiate(gameManager);
            curGameManager.InstanceRef = InstanceRef;
            curGameManager.LoadInstancesRef();
            curGameManager.prefabLoader.LoadPrefabs();
        }
    }
}
