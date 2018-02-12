using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMove : MonoBehaviour {

    public float Speed;

	void Update ()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }
}
