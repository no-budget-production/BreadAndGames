﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public SkillType BuffSkillType;

    [HideInInspector]
    public float Lifetime;

    [HideInInspector]
    public float duration;

    [HideInInspector]
    public Character Character;

    [HideInInspector]
    public Skill Skill;

    //public BuffType[] ThisBuffTypes;

    public virtual IEnumerator Fade()
    {
        float duration = Time.time + Lifetime;

        while (Time.time < duration)
        {
            Debug.Log(duration - Time.time);
            yield return null;
        }

        Character.ActiveBuffs.Remove(this);

        Destroy(gameObject);

        //for (int i = 0; i < ThisBuffTypes.Length; i++)
        //{
        //    bool hasActiveBuff = false;
        //    for (int j = 0; j < Character.ActiveBuffs.Count; j++)
        //    {
        //        if (hasActiveBuff)
        //        {
        //            break;
        //        }
        //        for (int k = 0; k < Character.ActiveBuffs[j].ThisBuffTypes.Length; k++)
        //        {
        //            if (ThisBuffTypes[i] == Character.ActiveBuffs[j].ThisBuffTypes[k])
        //            {
        //                hasActiveBuff = true;
        //                Debug.Log("Buff found: " + ThisBuffTypes[i]);
        //                break;
        //            }
        //        }
        //    }
        //    if (!hasActiveBuff)
        //    {
        //        Debug.Log("Not Buff found: " + ThisBuffTypes[i]);
        //        switch (ThisBuffTypes[i])
        //        {
        //            case BuffType.CanWalk:
        //                Character.canWalk = true;
        //                break;
        //            case BuffType.CanUseRightStick:
        //                Character.canUseRightStick = true;
        //                break;
        //            case BuffType.CanUseSkills:
        //                Character.canUseSkills = true;
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}
    }
}
