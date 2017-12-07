using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCollider : MonoBehaviour
{
    private HealthReference _healthReferenceScript;
    private GameObject _originGameObject;

    public LayerMask collisionMask;

    public float speed = 10;
    public float damage = 1;

    public float lifetime = 2;
    public float fadetime = 2;

    private float _bonusDamage;
    public float RayLengthMulti;

    public GameObject OriginGameObject
    {
        get { return _originGameObject; }
        set { _originGameObject = value; }
    }

    public float BonusDamage
    {
        get { return _bonusDamage; }
        set { _bonusDamage = value; }
    }

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
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance* RayLengthMulti, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
            Destroy(gameObject);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        Debug.Log(hit.collider.gameObject.name);
        if (hit.collider.gameObject.GetComponent("HealthReference") as HealthReference != null)
        {
            _healthReferenceScript = (HealthReference)hit.collider.gameObject.GetComponent(typeof(HealthReference));
            //_healthReferenceScript.LoseHealth(_bonusDamage);
            _healthReferenceScript.HealthScript.LoseHealth(_bonusDamage);
        }
        //hit.collider.gameObject.SendMessage("LoseHealth", _bonusDamage, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);

        //IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        //if (damageableObject != null)
        //{
        //    damageableObject.TakeHit(damage, hit);
        //}
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float fadePercent = 0;
        float fadeSpeed = 1 / fadetime;

        Material mat = GetComponent<Renderer>().material;
        Color initialColor = mat.color;

        while (fadePercent < 1)
        {
            fadePercent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColor, Color.clear, fadePercent);
            yield return null;
        }
        Destroy(gameObject);
    }

}
