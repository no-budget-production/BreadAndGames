using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour {

    public GameObject hitBox;
    public float damage = 10f;
        
    // Use this for initialization
    void Start ()
    {
        hitBox.GetComponent<Renderer>().enabled = true;
        hitBox.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        hitBox.SetActive(false);
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Gedrueckt");
            hitBox.SetActive(true);
        }
    }
}
