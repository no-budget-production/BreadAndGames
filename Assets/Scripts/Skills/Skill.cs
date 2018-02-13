using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{

    //public List<int> ReturnSelectedElements()
    //{
    //    List<int> selectedElements = new List<int>();
    //    for (int i = 0; i < System.Enum.GetValues(typeof(CooldownType)).Length; i++)
    //    {
    //        int layer = 1 << i;
    //        if (((int)ThisCoolDownType & layer) != 0)
    //        {
    //            selectedElements.Add(i);
    //        }
    //    }
    //    return selectedElements;
    //}

    //[EnumFlagsAttribute]
    //public CooldownType ThisCoolDownType;

    //public CooldownType[] UsedCoolDownTypes;

    //public bool HasFlag(CooldownType flags)
    //{
    //    int typeflag = 1 << (int)ThisCoolDownType;
    //    return (typeflag & (int)flags) != 0;
    //}

    //no G.O.
    public int SkillID;

    public GameObject Player;
    public PlayerController PlayerController;

    public bool isFiring;
    public Transform SkillSpawn;

    public Buff UsedBuff;
    public Buff curBuff;
    public float BuffDuration;

    public bool cantStack;

    public virtual void Shoot()
    {

    }

    public virtual void StopShoot()
    {

    }

    public void SpawnBuff()
    {
        if (UsedBuff == null)
        {
            return;
        }

        if (cantStack)
        {
            Debug.Log("CantStack");

            for (int i = 0; i < PlayerController.ActiveBuffs.Count; i++)
            {
                if (PlayerController.ActiveBuffs[i].BuffID == UsedBuff.BuffID)
                {
                    Debug.Log("Spawn Buff found: " + PlayerController.ActiveBuffs[i]);
                    Destroy(PlayerController.ActiveBuffs[i].gameObject);
                    PlayerController.ActiveBuffs.Remove(PlayerController.ActiveBuffs[i]);
                    break;
                }
            }
        }

        curBuff = Instantiate(UsedBuff, transform.position + UsedBuff.transform.position, Quaternion.identity);
        curBuff.transform.SetParent(transform);
        curBuff.PlayerController = PlayerController;
        curBuff.Lifetime = BuffDuration;
        PlayerController.ActiveBuffs.Add(curBuff);
    }
}
