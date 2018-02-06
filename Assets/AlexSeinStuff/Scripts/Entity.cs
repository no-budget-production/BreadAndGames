using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable, IHealable
{

    public float startingHealth;
    [SerializeField]
    protected float health;
    public ParticleSystem healEffect;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start ()
    {
        health = startingHealth;
        healEffect = GetComponent<ParticleSystem>();
	}
	
	public void TakeHit (float damage, RaycastHit hit)
    {
        health -= damage;

        if(health <= 0 && !dead)
        {
            Die();
        }
	}

    public void TakeHeal (float healAmount, RaycastHit hit)
    {
        if (health <= (health / startingHealth * 100) + 20 && !dead)
        {
            health += healAmount;
        }
    }

    protected void Die()
    {
        dead = true;
        GameObject.Destroy(gameObject);
    }
}
