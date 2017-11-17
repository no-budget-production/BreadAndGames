using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour {

    private HealthScript _healthScript;

    public float Lifetime;

    void Start()
    {
        Destroy(gameObject, Lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent("HealthScript") as HealthScript != null)
        {
            _healthScript = (HealthScript)other.GetComponent(typeof(HealthScript));
            _healthScript.LoseHealth(20.0f);
        }
        Destroy(gameObject);
    }
}
