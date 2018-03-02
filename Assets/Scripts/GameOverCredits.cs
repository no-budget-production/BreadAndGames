using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCredits : MonoBehaviour
{
    public Transform creditsObj;
    public float multi;

    public UIScript UIScript;

    private void Start()
    {
        Invoke("DelayedStatsMenu", 0f);
    }

    void Update()
    {
        creditsObj.Translate(Vector3.up * multi * Time.unscaledDeltaTime);
    }

    void DelayedStatsMenu()
    {
        UIScript.TimeScaleOff();
    }
}
