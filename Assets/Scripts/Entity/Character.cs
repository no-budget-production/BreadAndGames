using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveBuffObject
{
    public float BuffCurTime;
    public BuffObject BuffObject;
    public Character Character;

    public ActiveBuffObject(float buffCurTime, BuffObject buffObject, Character character)
    {
        this.BuffCurTime = buffCurTime;
        this.BuffObject = buffObject;
        this.Character = character;
    }
}

public class Character : Entity
{
    public RectTransform HealthBar;
    public bool UseHealthbar;

    public List<ActiveBuffObject> ActiveBuffObjects;

    public float MeleeDamage = 1f;
    public float RangeDamage = 1f;
    public float Accuracy = 1f;
    public float MoveSpeed;

    public float MeleeDamageMultiplicator = 1f;
    public float RangeDamageMultiplicator = 1f;
    public float AccuracyMultiplicator = 1f;
    public float MoveSpeedMultiplicator = 1f;

    public bool canWalk = true;
    public bool canUseRightStick = true;
    public bool canCurUseRightStick = true;
    public bool canUseSkills = true;

    protected virtual void Start()
    {
        OnChangeHealth(CurrentHealth);
    }

    public override void TakeDamage(float damage, DamageType damageType)
    {

        base.TakeDamage(damage, damageType);

        if (UseHealthbar)
        {
            OnChangeHealth(CurrentHealth);
        }
    }

    public override void GetHealth(float healing)
    {
        base.GetHealth(healing);

        if (UseHealthbar)
        {
            OnChangeHealth(CurrentHealth);
        }
    }

    void OnChangeHealth(float currentHealth)
    {
        HealthBar.sizeDelta = new Vector2(currentHealth / MaxHealth * 100, HealthBar.sizeDelta.y);
    }


    public virtual void Update()
    {
        UpdateBuffs();
    }

    public void AddBuff(BuffObject buff, int multi, Character character)
    {
        bool hasBuff = false;
        for (int i = 0; i < ActiveBuffObjects.Count; i++)
        {
            if (ActiveBuffObjects[i].BuffObject == buff)
            {
                hasBuff = true;

                //if (!buff.isStackable)
                {
                    //if (!buff.isPermanent)
                    {
                        if (multi < 0)
                        {
                            //BuffBuff(ActiveBuffObjects[i].BuffObject, -1);
                            //BuffEnd(ActiveBuffObjects[i].BuffObject, character);
                            Debug.Log("RemovingBuff " + i + " " + ActiveBuffObjects[i].BuffCurTime + " " + ActiveBuffObjects[i].BuffObject.name);
                            ActiveBuffObjects.RemoveAt(i);
                        }
                        else
                        {
                            ActiveBuffObjects[i].BuffCurTime = 0;
                            Debug.Log("ResetTime " + i + " " + ActiveBuffObjects[i].BuffCurTime + " " + ActiveBuffObjects[i].BuffObject.name);
                        }
                    }
                }
            }
        }

        if (!hasBuff)
        {
            BuffBuff(buff, multi);
        }

        if (multi > 0)
        {
            if (!hasBuff /* && !buff.isPermanent*/)
            {
                ActiveBuffObjects.Add(new ActiveBuffObject(0, buff, character));
                Debug.Log("AddingBuff " + buff.name);
            }
        }
    }

    void BuffBuff(BuffObject buff, int multi)
    {
        MeleeDamageMultiplicator += (buff.MeleeDamageMultiplicator * multi);
        RangeDamageMultiplicator += (buff.RangeDamageMultiplicator * multi);
        AccuracyMultiplicator += (buff.AccuracyMultiplicator * multi);
        RangeDamageMultiplicator += (buff.RangeDamageMultiplicator * multi);
        MeleeArmorMultiplicator += (buff.MeleeArmorMultiplicator * multi);
        RangeArmorMultiplicator += (buff.RangeArmorMultiplicator * multi);

        MoveSpeedMultiplicator += (buff.MoveSpeedMultiplicator * multi);
    }

    void BuffEnd(BuffObject buff, Character character)
    {
        if (buff.BuffEndScript != null)
        {
            BuffEndScript curBuffEndScript = Instantiate(buff.BuffEndScript, transform.position, Quaternion.identity);
            curBuffEndScript.transform.SetParent(transform);
            curBuffEndScript.Origion = character;
            curBuffEndScript.Parent = this;
            curBuffEndScript.EndScript();
        }
    }

    public bool HasBuff(BuffObject buffInQuestion)
    {
        for (int i = 0; i < ActiveBuffObjects.Count; i++)
        {
            if (ActiveBuffObjects[i].BuffObject == buffInQuestion)
            {
                return true;
            }
        }
        return false;
    }

    void UpdateBuffs()
    {
        if (ActiveBuffObjects.Count > 0)
        {
            int canWalkAgainCount = 0;
            int canUseRightStickAgainCount = 0;
            int canUseSkillsAgainCount = 0;
            bool expired = false;

            for (int i = 0; i < ActiveBuffObjects.Count; i++)
            {
                if (ActiveBuffObjects[i].BuffObject.LoseHealth > 0f)
                {
                    TakeDamage(ActiveBuffObjects[i].BuffObject.LoseHealth, ActiveBuffObjects[i].BuffObject.DamageType);
                }

                if (ActiveBuffObjects[i].BuffObject.GainHealth > 0f)
                {
                    GetHealth(ActiveBuffObjects[i].BuffObject.GainHealth);
                }

                if (ActiveBuffObjects[i].BuffObject.disableWalking)
                {
                    canWalkAgainCount++;
                }

                if (ActiveBuffObjects[i].BuffObject.disableRightStick)
                {
                    canUseRightStickAgainCount++;
                }

                if (ActiveBuffObjects[i].BuffObject.disableSkills)
                {
                    canUseSkillsAgainCount++;
                }

                if (!ActiveBuffObjects[i].BuffObject.isPermanent)
                {
                    ActiveBuffObjects[i].BuffCurTime += Time.deltaTime;

                    if (ActiveBuffObjects[i].BuffObject.maxTime < ActiveBuffObjects[i].BuffCurTime)
                    {
                        BuffBuff(ActiveBuffObjects[i].BuffObject, -1);
                        BuffEnd(ActiveBuffObjects[i].BuffObject, ActiveBuffObjects[i].Character);
                        ActiveBuffObjects.RemoveAt(i);
                        i--;

                        expired = true;
                        Debug.Log("RemovingBuff");
                        continue;
                    }
                }
            }

            if (expired)
            {
                if (!canWalk)
                {
                    if (canWalkAgainCount == 1)
                    {
                        canWalk = true;
                    }
                }
                if (canUseRightStick)
                {
                    if (!canCurUseRightStick)
                    {
                        if (canUseRightStickAgainCount == 1)
                        {
                            canUseRightStick = true;
                        }
                    }
                }
                if (!canUseSkills)
                {
                    if (canUseSkillsAgainCount == 1)
                    {
                        canUseSkills = true;
                    }
                }
            }
        }
    }
}