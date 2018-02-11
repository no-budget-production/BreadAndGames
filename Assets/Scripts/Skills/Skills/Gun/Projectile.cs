using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Effect
{
    public LayerMask collisionMask;

    //public List<int> ReturnSelectedElements()
    //{
    //    List<int> selectedElements = new List<int>();
    //    for (int i = 0; i < System.Enum.GetValues(typeof(TargetType)).Length; i++)
    //    {
    //        int layer = 1 << i;
    //        if (((int)ThisThisType & layer) != 0)
    //        {
    //            selectedElements.Add(i);
    //        }
    //    }
    //    return selectedElements;
    //}

    //[System.Flags]
    //public enum TargetType
    //{
    //    None, CoolDown0, CoolDown1, CoolDown2, CoolDown3
    //}

    //[EnumFlagsAttribute]
    //public TargetType ThisThisType;

    public UnitTypesFlags UnityTypeFlags;

    public float Speed = 10;
    public float Damage = 1;

    //float lifetime = 2;
    //float fadetime = 2;

    public void SetSpeed(float newSpeed)
    {
        Speed = newSpeed;

        StartCoroutine(Fade());
    }

    void Update()
    {
        float moveDistance = Speed * Time.deltaTime;
        CheckCollisions(moveDistance);

        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
    }

    void CheckCollisions(float moveDistance)
    {
        //Debug.Log("Check");
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
            GameObject.Destroy(gameObject);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        //Debug.Log(hit.collider.gameObject.name);
        Entity damageableObject = hit.collider.GetComponent<Entity>();
        if (damageableObject != null)
        {
            if (damageableObject.HasFlag(UnityTypeFlags))
            {
                damageableObject.TakeDamage(Damage);
            }

        }
    }
}
