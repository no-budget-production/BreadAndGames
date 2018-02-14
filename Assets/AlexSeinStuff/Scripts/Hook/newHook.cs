using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//design by hieu le
public class newHook : MonoBehaviour
{

    private Transform thisTransform;

    private List<Transform> hookObjects = new List<Transform> ();
    public float targetBondDamping = 10.0f;
    public float targetBondDistance = 0.2f;
    public float hookRotateSpeed = 500;

    public HookEventListener hookEventListender = new HookEventListener();

    private Transform hookModelTransform;
    private Transform ownerTransform;

    void SetOwnerTrans (Transform owner)
    {
        ownerTransform = owner;
	}
	
	// Update is called once per frame
	void Awake ()
    {
        thisTransform = transform;
        hookModelTransform = thisTransform.Find("hook");
	}

    void Update()
    {
        hookModelTransform.Rotate(Vector3.up * Time.deltaTime * hookRotateSpeed, Space.Self);
    }

    void OnDestroy ()
    {
        if (hookObjects.Count > 0)
        {
            if ( ownerTransform != null)
            {
                foreach ( Transform trans in hookObjects)
                {
                    Physics.IgnoreCollision(ownerTransform.GetComponent<Collider>(), trans.GetComponent<Collider>(), true);
                }
            }

            hookObjects.Clear();
        }
    }

    void FixedUpdate ()
    {
        if (hookObjects.Count > 0 && thisTransform != null)
        {
            foreach ( Transform trans in hookObjects)
            {
                FollowHook(thisTransform, trans);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "enemy")
        {
            hookObjects.Add(collider.gameObject.transform);
            {
                if (ownerTransform != null)
                {
                    

                    hookEventListender.NotifyTakeBack();
                }
                else if (collider.gameObject.tag == "cylinder")
                {
                    hookEventListender.NotifyHookSomething(thisTransform.position);
                }
            }
        }
    }

    private void FollowHook (Transform prevNode, Transform follower)
    {
        Quaternion targetRotation = Quaternion.LookRotation(prevNode.position - follower.position, prevNode.up);
        targetRotation.x = 0f;
        targetRotation.z = 0f;
        follower.rotation = Quaternion.Slerp(follower.rotation, targetRotation, Time.deltaTime * targetBondDamping);

        Vector3 targetPosition = prevNode.position;
        targetPosition -= follower.rotation * Vector3.forward * targetBondDistance;
        targetPosition.y = follower.position.y;
        follower.position = Vector3.Lerp(follower.position, targetPosition, Time.deltaTime * targetBondDamping);
    }
    
}
