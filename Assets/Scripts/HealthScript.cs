using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float health;
    public float healthMax;
    public float armor;

    public float Decay;

    private bool dead;

    public bool DestroyOnDeath;

    public void LoseHealth(float loseHealth){
        loseHealth -= armor;
        health -= Mathf.Clamp(loseHealth, 0, loseHealth);
        Debug.Log(health);

        if(health <= 0)
        {
            dead = true;

            if (DestroyOnDeath)
            {
                Destroy(gameObject, Decay);
            }            
        }

    }

    public void GainHealth(float gainHealth)
    {


        health += Mathf.Clamp(gainHealth, 0, gainHealth);
    }

    public void ArmorBuff(float armorArg)
    {
        armor += Mathf.Clamp(armorArg, 0, armorArg);
    }


}

