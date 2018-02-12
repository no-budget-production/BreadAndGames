using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameManager GameManager;
    public InstanceRef InstanceRef;

    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager curGameManager = Instantiate(GameManager);
            curGameManager.InstanceRef = InstanceRef;
            curGameManager.LoadInstancesRef();
            curGameManager.prefabLoader.LoadPrefabs();
        }
    }
}
