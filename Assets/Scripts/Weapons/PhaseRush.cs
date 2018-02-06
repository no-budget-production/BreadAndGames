using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseRush : MonoBehaviour
{

    private PlayerController charakter;
    public float moveSpeedBonus = 600;
    public float duration = 5;

    // Use this for initialization
    void Start()
    {
        charakter = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            StartPhaseRush();
        }
    }

    void StartPhaseRush()
    {
        Debug.Log(charakter.moveSpeed);
        charakter.moveSpeed += moveSpeedBonus;
        //charakter.IsDamageAble = false;
        Invoke("StopPhaseRush", duration);
        Debug.Log(charakter.moveSpeed);
    }

    void StopPhaseRush()
    {
        charakter.moveSpeed -= moveSpeedBonus;
        //charakter.IsDamageAble = true;
    }
}
