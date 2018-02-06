using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{

    public RectTransform healthBar;
    public bool UseHealthbar;

    protected virtual void Start()
    {
        OnChangeHealth(CurrentHealth);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (UseHealthbar)
        {
            OnChangeHealth(CurrentHealth);
        }
    }

    void OnChangeHealth(float currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth / MaxHealth * 100, healthBar.sizeDelta.y);
    }

}