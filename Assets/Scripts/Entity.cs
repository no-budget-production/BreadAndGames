using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable {

    public float CurrentHealth;
    public float MaxHealth;
    public float armor;

    public float Decay;

    public bool DestroyOnDeath;

    public RectTransform healthBar;
    public bool UseHealthbar;

    public bool IsDeadTrigger;

    protected virtual void Start()
    {
        OnChangeHealth(CurrentHealth);
        IsDeadTrigger = false;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        CurrentHealth -= damage;

        // Healthbar
        if (UseHealthbar)
        {
            OnChangeHealth(CurrentHealth);
        }

        // Death check
        if (CurrentHealth <= 0)
        {
            IsDeadTrigger = true;
            if (DestroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }

    public void TakePunch(float damage, Collider hit)
    {
        CurrentHealth -= damage;

        // Healthbar
        if (UseHealthbar)
        {
            OnChangeHealth(CurrentHealth);
        }

        // Death check
        if (CurrentHealth <= 0)
        {

            Debug.Log("DEAD");
            if (DestroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }



    public void ArmorBuff(float armorArg)
    {
        armor += Mathf.Clamp(armorArg, 0, armorArg);
    }

    void OnChangeHealth(float currentHealth)
    {
        
        healthBar.sizeDelta = new Vector2(currentHealth / MaxHealth * 100, healthBar.sizeDelta.y);
    }

}
