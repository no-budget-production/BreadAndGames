﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : Skill
{

    public SoundPlayer SoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds = 100;

    public float ReloadActionPointsAmount;
    public float SpendReloadAmount;

    public bool isReloadingFully;

    public override void Shoot()
    {
        if (!BuffObject.isStackable)
        {
            if (BuffObject.HasBuff(Character.ActiveBuffObjects))
            {
                return;
            }
        }

        if (BuffObject.HasCanTriggerWith(Character.ActiveBuffObjects))
        {
            return;
        }

        if (Character.curActionPoints >= Character.maxActionPoints)
        {
            return;
        }

        if (!Character.rechargeActionBarDirectly)
        {
            if (Character.curReloadBar < SpendReloadAmount)
            {
                Debug.Log("False");
                return;
            }
        }

        Debug.Log("True");

        if (Time.time > nextSoundTime)
        {
            nextSoundTime = Time.time + SBetweenSounds + SoundPlayer.GetClipLenght();
            SoundPlayer.Play();
        }

        Character.AddBuff(BuffObject, 1, Character);

        if (isReloadingFully)
        {
            Character.RestoreActionPoints(Character.maxActionPoints);
        }
        else
        {
            Character.RestoreActionPoints(ReloadActionPointsAmount);
        }

        Character.SpendReloads(SpendReloadAmount);

        SoundPlayer.Play();
    }
}
