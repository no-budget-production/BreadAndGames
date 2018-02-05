using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollider : MonoBehaviour {

    private float damage = 10;
    private GameObject Melee;

    void Start()
    {
        Melee = transform.parent.gameObject;
        Punch punch = Melee.GetComponent<Punch>();
        float damage = punch.damage;
    }

    void OnTriggerStay(Collider other)
    {
        IDamageable damageableObject = other.GetComponent<IDamageable>();
        Debug.Log(damageableObject);
        if (damageableObject != null)
        {
            damageableObject.TakePunch(damage, other);
        }
        print("space key was pressed and hit something");
    }
}
