using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    enum SkillType
    {
        Heal,
        Shoot
    }

    SkillType Type;

    //no G.O.
    public GameObject Player;
    public PlayerController PlayerController;
    //public GameObject[] ActivePlayers;
    public bool isFiring;
    public Transform SkillSpawn;

    public virtual void Shoot()
    {

    }
}
