using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWheel : MonoBehaviour {

    public GameObject Wheel;
    public GameObject Colllider;
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyUp("a"))
        {
            Wheel.SetActive(true);
            Colllider.SetActive(true);
        }
        if (Input.GetKeyUp("b"))
        {
            Wheel.SetActive(false);
            Colllider.SetActive(true);
        }
    }
}
