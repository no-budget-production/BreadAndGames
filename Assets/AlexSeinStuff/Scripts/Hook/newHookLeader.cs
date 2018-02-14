using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class newHookLeader : MonoBehaviour
{

    //enum hook status
    private enum HookStatus
    {
        shooting,
        takeback,
        reverseInit,
        reverse
    }

    private HookStatus hookStatus = HookStatus.shooting;

    public int maxNodes = 70;

    private List<GameObject> nodes = new List<GameObject>();

    private Transform thisTransform;
    public float bondDistance = 0.15f;
    public float bondDamping = 100f;

    public GameObject hookNodePrefab;
    public GameObject hookPrefab;
    private GameObject hook;
    private CharacterController ownerController;
    private Transform hookStartTransform;

    public float hookBondDistance = 0.25f;
    private float extendInterval = 0.05f;
    private float extendTime;
    public float takeBackInterval = 0f;
    private float takeBackTime;

    public int shootHookSpeed = 2;
    public int takeBackHookSpeed = 2;
    private int nodeCount;
    private float updateOrderTime;
    public float updateOrderInterval = 0f;
    public float UpdateOrderAngle = 110f;
    public int minUpdateOrderNum = 5;
    public int updateOrderSpeed = 2;
    private LineRenderer lineRenderer;
    private bool shouldKeepPosition;
    private Vector3 keepPosition;

    void StartTakeBackHook()
    {
        // pull back hook
        hookStatus = HookStatus.takeback;
    }

    void HookSomething(Vector3 hookContactPoint)
    {
        // hook something
        if (nodes.Count >= minUpdateOrderNum)
        {
            keepPosition = hookContactPoint;
            hookStatus = HookStatus.reverseInit;
        }
    }

    void HookLogic()
    {
        if (!shouldKeepPosition)
        {
            //keep hook leader follow owner position
            thisTransform.position = hookStartTransform.position;
        }

        // pull hook owner
        if (hookStatus == HookStatus.reverseInit)
        {
            shouldKeepPosition = true;

            thisTransform.position = keepPosition;
            //reverse nodes list
            nodes.Reverse();

            //ownerController.Control(false);
        }

        //hook shooting
        if (hookStatus == HookStatus.shooting)
        {
            if (nodeCount < maxNodes)
            {
                if (extendInterval > 0)
                {
                    if (Time.time - extendTime > extendInterval)
                    {
                        extendTime = Time.time;
                        addHookNode(shootHookSpeed);
                        nodeCount += shootHookSpeed;
                    }
                    else
                    {
                        addHookNode(shootHookSpeed);
                        nodeCount += shootHookSpeed;
                    }
                }
                else
                {
                    hookStatus = HookStatus.takeback;
                }
            }
        }

        //Adjust hook nodes transform when owner position change
        if (hookStatus != HookStatus.reverse && nodes.Count >= minUpdateOrderNum)
        {
            float angle = Quaternion.Angle(hookStartTransform.rotation, thisTransform.rotation);
            if (ownerController.IsMoving() && angle < UpdateOrderAngle)
            {
                bool updateNodeOrder = false;
                if (updateOrderInterval > 0)
                {
                    if (Time.time - updateOrderTime > updateOrderInterval)
                    {
                        updateOrderTime = Time.time;
                        updateNodeOrder = true;
                    }
                }
                else
                {
                    updateNodeOrder = true;
                }
                if (updateNodeOrder)
                {
                    takeBackHook(updateOrderSpeed);
                    addHookNode(updateOrderSpeed);
                }
            }
        }

        // pull back hook or reverse pulln back hook
        if (hookStatus == HookStatus.reverse && hook != null)
        {
            FollowPrev(hook.transform, ownerTransform);
        }
        if (nodes.Count <= 0)
        {
            Destroy(hook);
            Destroy(gameObject);
        }
    }

    private void addHookNode(int speed)
    {
        //new node bond
        for (int i = 0; i < speed; i++)
        {
            Transform preTransform = LastNode();

            Vector3 position = nextPosition(preTransform);
            Quaternion rotation = nextRotation(preTransform, position);
            GameObject hookNodeClone = Instantiate(hookNodePrefab, position, rotation) as GameObject;
            HookNode node = hookNodeClone.GetComponent<HookNode>();
            node.hookEventListener.StartTakeBackHook += StartTakeBackHook;

            Physics.IgnoreCollision(hookNodeClone.GetComponent<Collider>(), ownerTransform.gameObject.GetComponent<Collider>());
            if (nodes.Count < maxNodes)
            {
                nodes.Add(hookNodeClone);
            }
        }
    }
    private void takeBackHook(int speed)
    {
        // Remove node for take back hook chain
        for (int i = 0; i < speed; i++)
        {
            if (nodes.Count > 0)
            {
                HookNode node = nodes[0].GetComponent<HookNode>();
                node.RemoveMe();
                nodes.RemoveAt(0);
                if (nodes.Count == 0)
                {
                    break;
                }
            }
        }
    }
    private Transform LastNode()
    {
        if (nodes.Count > 0)
        {
            return nodes[nodes.Count - 1].transform;
        }
        else
        {
            return thisTransform;
        }
    }

    private Transform ownerTransform;
    public void Init(Transform shootPointTransform)
    {
        if (updateOrderSpeed > minUpdateOrderNum)
        {
            updateOrderSpeed = minUpdateOrderNum;
            //throw new System.ArgumentException("updateOrderSpeed can not be greater than minUpdateOrderNum");
        }

        thisTransform = transform;
        this.ownerTransform = shootPointTransform.parent;//ownerTransform;

        ownerController = ownerTransform.GetComponent<CharacterController>();
        hookStartTransform = shootPointTransform;//ownerTransform.gameObject.transform.FindChild("HookShootPoint");

        // Instantiate hook
        Vector3 position = nextPosition(thisTransform.transform);
        Quaternion rotation = nextRotation(thisTransform.transform, position);
        hook = Instantiate(hookPrefab, position, rotation) as GameObject;
        Hook hookScript = hook.GetComponent<Hook>();
        hookScript.setOwnerTrans(ownerTransform);
        hookScript.hookEventListener.StartTakeBackHook += StartTakeBackHook;
        hookScript.hookEventListener.HookSomething += HookSomething;

        Physics.IgnoreCollision(hook.GetComponent<Collider>(), ownerTransform.gameObject.GetComponent<Collider>());

        // Slow down owner speed
        ownerController.SlowDownMovingSpeed();
    }

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer)
            lineRenderer.enabled = false;
    }

    void Start()
    {

    }

    void OnDestroy()
    {
        // Recover owner speed
        ownerController.NormalMovingSpeed();
        // Owner can be controlled
        ownerController.Control(true);
    }

    void FixedUpdate()
    {

        HookLogic();

        // update hook nodes transform
        for (int i = 0; i < nodes.Count; i++)
        {
            FollowPrev(i == 0 ? thisTransform : nodes[i - 1].transform, nodes[i].transform);
        }

        // update hook transform
        if (hook != null)
        {
            HookFollowLast(LastNode(), hook.transform);
        }

        // Renderer hook path
        if (lineRenderer && nodes.Count >= 5)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetVertexCount(nodes.Count);
            for (int i = 0; i < nodes.Count; i++)
            {
                lineRenderer.SetPosition(i, nodes[i].transform.position);
            }
        }

    }

    private Vector3 nextPosition(Transform prevNode)
    {
        // Get next node position

        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, prevNode.eulerAngles.y, 0);

        Vector3 position = prevNode.position;
        position -= currentRotation * Vector3.forward * bondDistance;
        return position;
    }
    private Quaternion nextRotation(Transform prevNode, Vector3 position)
    {
        // Get next node rotation
        return Quaternion.LookRotation(prevNode.position - position, prevNode.up);
    }

    private void HookFollowLast(Transform prevNode, Transform node)
    {

        float targetRotationAngle = prevNode.eulerAngles.y;
        float currentRotationAngle = node.transform.eulerAngles.y;
        // Calculate the current rotation angles
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, bondDamping * Time.deltaTime);
        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        // bondDistance meters behind the prevNode
        node.transform.position = prevNode.position;
        node.transform.position -= currentRotation * Vector3.forward * hookBondDistance;
        //Always look at the prevNode
        node.transform.LookAt(prevNode);
    }

    private void FollowPrev(Transform prevNode, Transform node)
    {
        // Set node's rotation and position by the previous node

        Quaternion targetRotation = Quaternion.LookRotation(prevNode.position - node.position, prevNode.up);
        targetRotation.x = 0;
        targetRotation.z = 0;
        node.rotation = Quaternion.Slerp(node.rotation, targetRotation, Time.deltaTime * bondDamping);

        Vector3 targetPosition = prevNode.position;
        targetPosition -= node.transform.rotation * Vector3.forward * bondDistance;
        targetPosition.y = node.position.y;
        node.position = Vector3.Lerp(node.position, targetPosition, Time.deltaTime * bondDamping);
    }
}




