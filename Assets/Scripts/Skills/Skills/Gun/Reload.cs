using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : Skill
{

    public SoundPlayer SoundPlayer;
    private float nextSoundTime;
    public float SBetweenSounds = 100;

    public override void Shoot()
    {
        if (BuffObject.HasBuff(Character.ActiveBuffObjects))
        {
            return;
        }

        if (BuffObject.HasCanTriggerWith(Character.ActiveBuffObjects))
        {
            return;
        }


        if (Time.time > nextSoundTime)
        {
            nextSoundTime = Time.time + SBetweenSounds + SoundPlayer.GetClipLenght();
            SoundPlayer.Play();
        }

        Character.AddBuff(BuffObject, 1, Character);

        if (Character.curActionPoints < Character.maxActionPoints)
        {
            Character.curActionPoints = Character.maxActionPoints;
            Character.OnActionBarChange();
            SoundPlayer.Play();
        }
    }
}
