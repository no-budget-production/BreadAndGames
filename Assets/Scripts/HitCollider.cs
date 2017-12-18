using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour {

    private HealthReference _healthReferenceScript;
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
        if (other.GetComponent("HealthReference") as HealthReference != null)
        {
            _healthReferenceScript = (HealthReference)other.GetComponent(typeof(HealthReference));
            //_healthReferenceScript.LoseHealth(_bonusDamage);
            _healthReferenceScript.playerControllerScript.LoseHealth(_bonusDamage);
        }
        Destroy(gameObject);
    }


}
