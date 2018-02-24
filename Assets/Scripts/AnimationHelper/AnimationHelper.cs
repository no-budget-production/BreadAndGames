using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{

    public Character Character;

    public void SkillHit(int Skillnumber)
    {
        Debug.Log("AnimationHelper Skill_" + Skillnumber);
        Character.SkillHit(Skillnumber);
    }
}
