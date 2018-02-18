using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public SkillType BuffSkillType;

    [HideInInspector]
    public float Lifetime;

    public PlayerController PlayerController;

    public BuffType[] ThisBuffTypes;

    public virtual IEnumerator Fade()
    {
        yield return new WaitForSeconds(Lifetime);

        PlayerController.ActiveBuffs.Remove(this);

        Destroy(this.gameObject);

        for (int i = 0; i < ThisBuffTypes.Length; i++)
        {
            bool hasActiveBuff = false;
            for (int j = 0; j < PlayerController.ActiveBuffs.Count; j++)
            {
                if (hasActiveBuff)
                {
                    break;
                }
                for (int k = 0; k < PlayerController.ActiveBuffs[j].ThisBuffTypes.Length; k++)
                {
                    if (ThisBuffTypes[i] == PlayerController.ActiveBuffs[j].ThisBuffTypes[k])
                    {
                        hasActiveBuff = true;
                        Debug.Log("Buff found: " + ThisBuffTypes[i]);
                        break;
                    }
                }
            }
            if (!hasActiveBuff)
            {
                Debug.Log("Not Buff found: " + ThisBuffTypes[i]);
                switch (ThisBuffTypes[i])
                {
                    case BuffType.CanWalk:
                        PlayerController.canWalk = true;
                        break;
                    case BuffType.CanUseRightStick:
                        PlayerController.canUseRightStick = true;
                        break;
                    case BuffType.CanUseSkills:
                        PlayerController.canUseSkills = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
