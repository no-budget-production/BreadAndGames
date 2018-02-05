using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollider : MonoBehaviour {
    
    private GameObject Melee; //Player
    public List<SwarmController> enemies; //Enemy

    void Start()
    {
        Melee = transform.parent.gameObject;
    }

    void OnTriggerEnter(Collider other) 
    {
        var temp = GetComponent<SwarmController>();
        if (temp == null) return;
        enemies.Add(temp);
    }

    void OnTriggerLeave(Collider other)
    {
        var temp = GetComponent<SwarmController>();
        if (temp == null) return;
        enemies.Remove(temp);
    }
}
