using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float CurrentHealth;
    public float MaxHealth;
    public float armor;

    public float Decay;

    public bool DestroyOnDeath;

    public RectTransform healthBar;
    public bool UseHealthbar;

    private void Start()
    {
        OnChangeHealth(CurrentHealth);
    }

    public void LoseHealth(float loseHealth){
        if (CurrentHealth >= 0)
        {
            loseHealth -= armor;
            CurrentHealth -= Mathf.Clamp(loseHealth, 0, loseHealth);
            if (UseHealthbar)
            {
                OnChangeHealth(CurrentHealth);
            }

            Debug.Log(CurrentHealth);

            if (CurrentHealth <= 0)
            {
                if (DestroyOnDeath)
                {
                    Destroy(gameObject, Decay);
                }
            }
        }    
    }

    public void GainHealth(float gainHealth)
    {
        CurrentHealth += Mathf.Clamp(gainHealth, 0, gainHealth);
        if (UseHealthbar)
        {
            OnChangeHealth(CurrentHealth);
        }

        Debug.Log(CurrentHealth);
    }

    public void ArmorBuff(float armorArg)
    {
        armor += Mathf.Clamp(armorArg, 0, armorArg);
    }

    void OnChangeHealth(float currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth/MaxHealth*100, healthBar.sizeDelta.y);
    }

}

