using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : Skill
{
    public AudioSource AudioSource;
    public AudioClip SoundReload;

    public override void Shoot()
    {
        if (base.PlayerController.curActionPoints < base.PlayerController.ActionPoints - 1)
        {
            base.PlayerController.isInAction = true;
            AudioSource.PlayOneShot(SoundReload);
            base.PlayerController.curActionPoints = base.PlayerController.ActionPoints;
            //base.ActionCD = Time.time + reloadTime;

            base.PlayerController.isInAction = false;
        }
    }

}
