using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWheel : MonoBehaviour
{

    public GameObject Wheel;
    public GameObject Colllider;
    public GameObject Heart;

    public void Activate()
    {
        Wheel.SetActive(true);
        Colllider.SetActive(true);
        Heart.SetActive(true);
    }
    public void Deactivate()
    {
        Wheel.SetActive(false);
        Colllider.SetActive(false);
        Heart.SetActive(false);
    }
}
