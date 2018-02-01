using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealable
{
    void TakeHeal(float healAmount, RaycastHit hit);
}
