using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour {

    private Health _healthScript;
    private GameObject _originGameObject;

    private float _bonusDamage;

    public float Lifetime;

    public GameObject OriginGameObject
    {
        get { return _originGameObject; }
        set { _originGameObject = value; }
    }

    public float BonusDamage
    {
        get { return _bonusDamage; }
        set { _bonusDamage = value; }
    }

    void Start()
    {

        Destroy(gameObject, Lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent("Health") as Health != null)
        {
            _healthScript = (Health)other.GetComponent(typeof(Health));
            _healthScript.LoseHealth(_bonusDamage);
        }
        Destroy(gameObject);
    }


}
