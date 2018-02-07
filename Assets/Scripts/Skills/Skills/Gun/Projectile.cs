using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Effect
{
    public LayerMask collisionMask;

    private float speed = 10;
    private float damage = 1;

    //float lifetime = 2;
    //float fadetime = 2;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;

        StartCoroutine(Fade());
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
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
            damageableObject.TakeDamage(damage);
        }
    }
}
