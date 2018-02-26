using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [Header(">>>>>>>>>> Character:")]

    public bool NPC = true;

    public float maxActionPoints;
    public float curActionPoints;

    public float maxReloadBar;
    public float curReloadBar;

    public float ActionPointRegeneration;
    public float ReloadRegeneration;

    public float MeleeDamage = 1f;
    public float RangeDamage = 1f;
    public float Accuracy = 1f;
    public float MoveSpeed;

    //[HideInInspector]
    public float MeleeDamageMultiplicator = 1f;
    //[HideInInspector]
    public float RangeDamageMultiplicator = 1f;
    //[HideInInspector]
    public float AccuracyMultiplicator = 1f;
    //[HideInInspector]
    public float MoveSpeedMultiplicator = 1f;
    //[HideInInspector]
    public float ActionPointMultiplicator = 1f;
    //[HideInInspector]
    public float ActionPointRegenerationMultiplicator = 1f;
    //[HideInInspector]
    public float ReloadRegenerationMultiplicator = 1f;

    public bool canWalk = true;
    public bool canUseRightStick = true;
    public bool canNeverUseRightStick = true;
    public bool canUseSkills = true;

    //[HideInInspector]
    public Transform SkillSpawn;
    //[HideInInspector]
    public Transform TakeHitPoint;

    //[HideInInspector]
    public RectTransform HealthBar;
    public bool UseHealthbar;

    [HideInInspector]
    public Slider HUDHealthBarSlider;
    public bool UseHUDHealthbarSlider;

    //[HideInInspector]
    public Slider ActionPointsBar;
    public bool UseActionPointsBar;

    [HideInInspector]
    public Slider HUDActionPointsBar;
    public bool UseHUDActionPointsBar;

    [HideInInspector]
    public Slider OverChargeBar;
    public bool UseOverChargeBar;
    public float maxOverCharge;
    public float curOverCharge;

    [HideInInspector]
    public Slider ReloadBar;
    public bool UseReloadBar;
    public float curDisplaySteps;
    public int curDisplayReloadBar;

    public bool rechargeActionBarDirectly;

    //[HideInInspector]
    public MultiSoundPlayer MultiSoundPlayer;

    public List<ActiveBuffObject> ActiveBuffObjects;

    private float nextSoundTime;
    public float SBetweenSounds = 1;

    public Skill[] ActiveSkills;

    public List<Buff> ActiveBuffs;

    //[HideInInspector]
    public Animator _Animtor;


    public virtual void Disable()
    {
        for (int i = 0; i < ActiveBuffObjects.Count; i++)
        {
            AddBuff(ActiveBuffObjects[i].BuffObject, -1, GetComponent<Character>());
        }

        canWalk = false;
        canUseRightStick = false;
        canUseSkills = false;

        for (int i = 0; i < ActiveBuffs.Count; i++)
        {
            Destroy(ActiveBuffs[i].GetComponent<GameObject>());
        }

        ActiveBuffs.Clear();
    }


    public virtual void Enable()
    {
        canWalk = true;
        if (canNeverUseRightStick)
        {
            canUseRightStick = true;
        }
        canUseSkills = true;
    }

    public override void OnCustomDestroy()
    {
        base.OnCustomDestroy();

        if (isDeadTrigger)
        {
            for (int i = 0; i < ActiveSkills.Length; i++)
            {
                if (ActiveSkills[i].FireOnDeath)
                {
                    ActiveSkills[i].Shoot();
                }
            }
            Disable();
        }
    }

    public virtual void Start()
    {
        CurrentHealth = MaxHealth;
        curActionPoints = maxActionPoints;


        if (UseHealthbar)
        {
            OnChangeHealth(CurrentHealth);
        }
        if (UseActionPointsBar)
        {
            OnActionBarChange();
        }
        if (UseHUDHealthbarSlider)
        {
            OnHUDChangeHealthSlider();
        }
        if (UseReloadBar)
        {
            if (curDisplaySteps != 0)
            {
                curDisplayReloadBar = Mathf.RoundToInt(maxReloadBar / curDisplaySteps);
            }
            OnChangeReloadSlider();
        }
        if (UseOverChargeBar)
        {
            OnChangeOverchargeSlider();
        }
    }


    public override void TakeDamage(float damage, DamageType damageType)
    {
        base.TakeDamage(damage, damageType);

        if (isDeadTrigger)
        {
            Disable();
        }

        if (!(hasDied && DestroyOnDeath))
        {
            if (UseHealthbar)
            {
                OnChangeHealth(CurrentHealth);
            }

            if (UseHUDHealthbarSlider)
            {
                OnHUDChangeHealthSlider();
            }
        }
    }

    public override void GetHealth(float healing)
    {
        base.GetHealth(healing);

        if (!(hasDied && DestroyOnDeath))
        {
            if (UseHealthbar)
            {
                OnChangeHealth(CurrentHealth);
            }

            if (UseHUDHealthbarSlider)
            {
                OnHUDChangeHealthSlider();
            }
        }

        if (!isDeadTrigger)
        {
            Enable();
        }

    }

    void OnChangeHealth(float currentHealth)
    {
        HealthBar.sizeDelta = new Vector2(currentHealth / MaxHealth * 100, HealthBar.sizeDelta.y);
    }


    public virtual void Update()
    {
        Regenerate();
        UpdateBuffs();
    }

    void Regenerate()
    {
        if (!isDeadTrigger)
        {
            GetHealth(HealthRegeneration * HealthRegenerationMultiplicator * Time.deltaTime);
            RestoreActionPoints(ActionPointRegeneration * ActionPointRegenerationMultiplicator * Time.deltaTime);
            RestoreReloadPoints(ReloadRegeneration * ReloadRegenerationMultiplicator * Time.deltaTime);

        }
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
                            BuffBuff(ActiveBuffObjects[i].BuffObject, -1);
                            //BuffEnd(ActiveBuffObjects[i].BuffObject, character);
                            //Debug.Log("RemovingBuff " + i + " " + ActiveBuffObjects[i].BuffCurTime + " " + ActiveBuffObjects[i].BuffObject.name);
                            ActiveBuffObjects.RemoveAt(i);
                        }
                        else
                        {
                            ActiveBuffObjects[i].BuffCurTime = 0;
                            //Debug.Log("ResetTime " + i + " " + ActiveBuffObjects[i].BuffCurTime + " " + ActiveBuffObjects[i].BuffObject.name);
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
                //Debug.Log("AddingBuff " + buff.name);
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

        HealthRegenerationMultiplicator += (buff.HealthRegenerationMultiplicator * multi);
        ActionPointRegeneration += (buff.ActionPointRegeneration * multi);

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
                    TakeDamage(ActiveBuffObjects[i].BuffObject.LoseHealth * Time.deltaTime, ActiveBuffObjects[i].BuffObject.DamageType);
                }

                if (ActiveBuffObjects[i].BuffObject.GainHealth > 0f)
                {
                    GetHealth(ActiveBuffObjects[i].BuffObject.GainHealth);
                }

                //
                if (ActiveBuffObjects[i].BuffObject.GainActionPoints > 0f)
                {
                    RestoreActionPoints(ActiveBuffObjects[i].BuffObject.GainActionPoints * Time.deltaTime);
                }

                if (ActiveBuffObjects[i].BuffObject.GainHealth > 0f)
                {
                    SpendActionPoints(ActiveBuffObjects[i].BuffObject.LoseActionPoints * Time.deltaTime);
                }
                //

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

                        //Debug.Log("RemovingBuff");
                        continue;
                    }
                }
            }

            //if (expired)
            {
                if (!canWalk)
                {
                    if (canWalkAgainCount <= 0)
                    {
                        canWalk = true;
                    }
                }
                if (canUseRightStick)
                {
                    if (!canNeverUseRightStick)
                    {
                        if (canUseRightStickAgainCount <= 0)
                        {
                            canUseRightStick = true;
                        }
                    }
                }
                if (!canUseSkills)
                {
                    if (canUseSkillsAgainCount <= 0)
                    {
                        canUseSkills = true;
                    }
                }
            }
        }
    }

    public void SpendActionPoints(float costs)
    {
        curActionPoints = Mathf.Max(curActionPoints - (costs), 0);

        if (UseActionPointsBar)
        {
            OnActionBarChange();
        }

        if (UseHUDActionPointsBar)
        {
            OnHUDActionBarChange();
        }
    }

    public virtual float RestoreActionPoints(float restore)
    {

        float tempActionPoints = Mathf.Min(curActionPoints + (restore), maxActionPoints);
        float dif = tempActionPoints - curActionPoints;
        curActionPoints = tempActionPoints;

        if (UseActionPointsBar)
        {
            OnActionBarChange();
        }

        if (UseHUDActionPointsBar)
        {
            OnHUDActionBarChange();
        }

        return dif;

    }

    public virtual float RestoreReloadPoints(float restore)
    {

        float tempReloadPoints = Mathf.Min(curReloadBar + (restore), maxReloadBar);
        float dif = tempReloadPoints - curReloadBar;
        curReloadBar = tempReloadPoints;

        if (UseReloadBar)
        {
            RoundDisplayBar();
            OnChangeReloadSlider();
        }
        return dif;

    }

    public void SpendReloads(float costs)
    {
        curReloadBar = Mathf.Max(curReloadBar - (costs), 0);

        RoundDisplayBar();

        OnChangeReloadSlider();
    }

    void RoundDisplayBar()
    {
        curDisplayReloadBar = Mathf.RoundToInt(curReloadBar / curDisplaySteps);
    }

    public void EmptySound()
    {
        if (MultiSoundPlayer != null)
        {
            if (Time.time > nextSoundTime)
            {
                MultiSoundPlayer.EmptySound();
                nextSoundTime = Time.time + SBetweenSounds + MultiSoundPlayer.GetClipLenght();
            }
        }
    }

    public void OnActionBarChange()
    {
        ActionPointsBar.value = curActionPoints;
    }

    public void OnHUDActionBarChange()
    {
        HUDActionPointsBar.value = curActionPoints;
    }

    public void OnHUDChangeHealthSlider()
    {
        HUDHealthBarSlider.value = CurrentHealth;
    }

    public void OnChangeOverchargeSlider()
    {
        if (UseOverChargeBar)
        {
            OverChargeBar.value = curOverCharge;
        }
    }

    public void OnChangeReloadSlider()
    {
        if (UseReloadBar)
        {
            ReloadBar.value = curDisplayReloadBar;
        }
    }

    public virtual void SkillHit(int Skillnumber)
    {
        //Debug.Log("ARRIVED AnimationHelper Skill_" + Skillnumber);
        ActiveSkills[Skillnumber].SkillHit();
    }
}