using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{

    public List<int> ReturnSelectedElements()
    {

        List<int> selectedElements = new List<int>();
        for (int i = 0; i < System.Enum.GetValues(typeof(CooldownType)).Length; i++)
        {
            int layer = 1 << i;
            if (((int)ThisCoolDownType & layer) != 0)
            {
                selectedElements.Add(i);
            }
        }
        return selectedElements;
    }

    [System.Flags]
    public enum CooldownType
    {
        None, CoolDown0, CoolDown1, CoolDown2, CoolDown3
    }

    [EnumFlagsAttribute]
    public CooldownType ThisCoolDownType;

    //public enum SkillType
    //{
    //    CoolDown0,
    //    CoolDown1,
    //    CoolDown2,
    //    CoolDown3
    //}

    //[System.Flags]
    //public enum SkillTypeFlags
    //{
    //    CoolDown0 = 1 << SkillType.CoolDown0,
    //    CoolDown1 = 1 << SkillType.CoolDown1,
    //    CoolDown2 = 1 << SkillType.CoolDown2,
    //    CoolDown3 = 1 << SkillType.CoolDown3
    //}

    public bool HasFlag(CooldownType flags)
    {
        int typeflag = 1 << (int)ThisCoolDownType;
        return (typeflag & (int)flags) != 0;
    }

    //[SerializeField]
    //public SkillType SkillTypes;

    //no G.O.
    public GameObject Player;
    public PlayerController PlayerController;
    //public GameObject[] ActivePlayers;
    public bool isFiring;
    public Transform SkillSpawn;

    public virtual void Shoot()
    {

    }

    public virtual void StopShoot()
    {

    }
}
