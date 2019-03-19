using UnityEngine;

public class EnergyBattery : Character
{
    [Header(">>>>>>>>>> EnergyBattery:")]

    public Skill[] UsedSkills;

    private Vector3 direction;

    public MeshRenderer[] batteryMeshRenderers;
    public Material EmptyMaterial;
    public Material FullMaterial;

    public Transform Battery;

    public float tweak;

    public bool canRegenerateEnergy;
    public float startRecharge;
    public float stopRecharge;

    public float startRayBlocking;

    public override void Start()
    {
        base.Start();

        canRegenerateEnergy = true;

        SkillSetup();
    }

    public void SkillSetup()
    {
        ActiveSkills = new Skill[UsedSkills.Length];

        for (int i = 0; i < UsedSkills.Length; i++)
        {
            Skill curSkill = Instantiate(UsedSkills[i], thisTransform.position + UsedSkills[i].transform.position, Quaternion.identity, thisTransform);
            //curSkill.transform.SetParent(transform);
            curSkill.Character = this;
            curSkill.SkillSpawn = SkillSpawn;
            ActiveSkills[i] = curSkill;
        }

        for (int i = 0; i < ActiveSkills.Length; i++)
        {
            ActiveSkills[i].LateSkillSetup();
        }
    }

    public override void Update()
    {
        base.Update();

        BatteryFunction();

        if (canRegenerateEnergy)
        {
            for (int i = 0; i < ActiveSkills.Length; i++)
            {
                ActiveSkills[i].Shoot();
            }
        }
    }

    bool InRange(Transform transformTarget, float MaxRange)
    {
        if (Vector3.Distance(transformTarget.position, thisTransform.position) > MaxRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void BatteryFunction()
    {
        if (curActionPoints >= startRayBlocking)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        if (curActionPoints >= startRecharge)
        {
            canRegenerateEnergy = true;
            for (int i = 0; i < batteryMeshRenderers.Length; i++)
            {
                batteryMeshRenderers[i].material = FullMaterial;
            }
        }
        else if (curActionPoints <= stopRecharge)
        {
            canRegenerateEnergy = false;
            for (int i = 0; i < batteryMeshRenderers.Length; i++)
            {
                batteryMeshRenderers[i].material = EmptyMaterial;
            }

        }
        Battery.position = new Vector3(Battery.position.x, curActionPoints * tweak, Battery.position.z);
    }
}