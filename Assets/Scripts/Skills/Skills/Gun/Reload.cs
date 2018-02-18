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
        if (BuffObject.HasBuff(PlayerController.ActiveBuffObjects))
        {
            return;
        }

        if (BuffObject.HasCanTriggerWith(PlayerController.ActiveBuffObjects))
        {
            return;
        }


        if (Time.time > nextSoundTime)
        {
            nextSoundTime = Time.time + SBetweenSounds + SoundPlayer.GetClipLenght();
            SoundPlayer.Play();
        }

        PlayerController.AddBuff(BuffObject, 1, PlayerController);

        if (PlayerController.curActionPoints < PlayerController.ActionPoints)
        {
            PlayerController.isInAction = true;
            PlayerController.curActionPoints = PlayerController.ActionPoints;
            PlayerController.OnActionBarChange();
            SoundPlayer.Play();
            PlayerController.isInAction = false;
        }

    }
}
