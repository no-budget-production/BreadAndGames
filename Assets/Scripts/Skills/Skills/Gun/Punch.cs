using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour {

    private PunchCollider hitBox;
    public float damage = 10f;
    public float cooldown = 10;

    // Use this for initialization
    void Start ()
    {
        hitBox = GetComponentInChildren<PunchCollider>();
        hitBox.GetComponent<Renderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Punch");
            hitBox.enemies.ForEach(e => e.TakeDamage(damage));
        }
    }
}
