using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollider : MonoBehaviour {
    
    private GameObject Melee;

    void Start()
    {
        Melee = transform.parent.gameObject;
    }

    void OnTriggerStay(Collider other) 
    {
        Punch punch = Melee.GetComponent<Punch>();
        IDamageable damageableObject = other.GetComponent<IDamageable>();
        Debug.Log(damageableObject);
        if (damageableObject != null)
        {
            damageableObject.TakePunch(punch.damage, other);
        }
        print("space key was pressed and hit something");
    }
}
