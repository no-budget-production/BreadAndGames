using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentationCamera : MonoBehaviour {
    
    public float speed = 10f;
    Vector3 tempPos;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        tempPos = transform.position;
        if (Input.GetKey("left")) {
            tempPos.x -= speed;
            transform.position = tempPos;
        }
        if (Input.GetKey("right")) {
            tempPos.x += speed;
            transform.position = tempPos;
        }

    }
}
