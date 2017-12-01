using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{

    public float startingHealth;
    [SerializeField]
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start ()
    {
        health = startingHealth;
	}
	
	public void TakeHit (float damage, RaycastHit hit)
    {
        health -= damage;

        if(health <= 0 && !dead)
        {
            Die();
        }
	}

    protected void Die()
    {
        dead = true;
        GameObject.Destroy(gameObject);
    }
}
