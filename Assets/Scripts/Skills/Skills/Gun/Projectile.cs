using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Effect
{
    public LayerMask collisionMask;

    public int EveryXFrames;
    private int FrameCounter;

    public List<int> ReturnSelectedElements()
    {
        List<int> selectedElements = new List<int>();
        for (int i = 0; i < System.Enum.GetValues(typeof(UnitTypes)).Length; i++)
        {
            int layer = 1 << i;
            if (((int)ThisUnityTypeFlags & layer) != 0)
            {
                selectedElements.Add(i);
            }
        }
        return selectedElements;
    }

    [EnumFlagsAttribute]
    public UnitTypesFlags ThisUnityTypeFlags;

    public DamageType DamageType;

    public Character Shooter;

    public float Speed = 10;
    public float Damage = 1;


    private bool shieldImmunity = false;

    //float lifetime = 2;
    //float fadetime = 2;

    public void shieldImmunize()
    {
        shieldImmunity = true;
    }

    public void SetSpeed(float newSpeed)
    {
        Speed = newSpeed;

        StartCoroutine(Fade());
    }

    void Update()
    {
        float moveDistance = Speed * Time.deltaTime;

        FrameCounter++;
        if ((FrameCounter % EveryXFrames) == 0)
        {
            CheckCollisions(moveDistance);
            FrameCounter = 0;
        }

        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
    }

    void CheckCollisions(float moveDistance)
    {
        //Debug.Log("Check");
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance * EveryXFrames, collisionMask, QueryTriggerInteraction.Collide))
        {
            if (OnHitObject(hit))
                Destroy(this.gameObject);
        }
    }

    bool OnHitObject(RaycastHit hit)
    {
        //Debug.Log(hit.collider.gameObject.name);
        Entity damageableObject = hit.collider.GetComponent<Entity>();
        if (damageableObject != null)
        {
            if (FlagsHelper.HasUnitTypes(damageableObject.ThisUnityTypeFlags, ThisUnityTypeFlags))
            {
                damageableObject.TakeDamage(Shooter.RangeDamage * Shooter.RangeDamageMultiplicator * Damage, DamageType);
                return true;
                //Debug.Log("TakeDamage");
            }
        }

        MeleeShieldHandler MeleeShield = hit.collider.GetComponentInParent<MeleeShieldHandler>();
        if (MeleeShield != null && !shieldImmunity)
        {
            return MeleeShield.AddProjectile(this);

        }
        return true;
    }
}
