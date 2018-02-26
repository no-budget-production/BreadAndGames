using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCredits : MonoBehaviour
{
    public Transform creditsObj;
    public float multi;

    

	void Update ()
    {
        creditsObj.Translate(Vector3.up * multi * Time.deltaTime);

    }
}
