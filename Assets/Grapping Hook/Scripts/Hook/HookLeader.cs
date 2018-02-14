using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HookLeader : MonoBehaviour
{

	// Hook status
	private enum HookStatus
	{
		shooting, 
		takeback,
		reverseInit,
		reverse
	} 
	private HookStatus hookStatus = HookStatus.shooting;		// Hook is shooting state when it was created

	public int maxNodes = 70; 									// Max number of nodes that can be in the chain

	private List<GameObject> nodes = new List<GameObject> ();	// Nodes in the chain

	private Transform mTransform;
	public float bondDistance = 0.15f;							// Distance between nodes
	public float bondDamping = 100f;							// The damping for bond node

	public GameObject hookNodePrefab;							// Node Prefab
	public GameObject hookPrefab;								// Hook Prefab
	private GameObject hook;									// Hook gameObject
	private CharacterController ownerController;			// Owner controller
	private Transform hookStartTransform;						// The transform of hook start 

	public float hookBondDistance = 0.25f;						// Distance between hook and last hook node.


	public float extendInterval = 0.05f;						// Bond node interval
	private float extendTime;									// Time last node bond
	public float takeBackInterval = 0f;							// Interval take back hook node 
	private float takeBackTime;									// Time last node tack back 
	public int shootHookSpeed = 1;								// Bond hook node number every time
	public int takeBackHookSpeed = 2;							// Take back hook node number every time
	private int nodeCount;										// Current count of hook node 
	private float updateOrderTime;								// Time Last update node order
	public float updateOrderInterval = 0f;						// Interval update node order
	public float updateOrderAngle = 110f;						// The angle between owner and hook chain
	public int minUpdateOrderNum = 5;							// Update node order when node count > this parameter
	public int updateOrderSpeed = 2;							// Update hook node number every time
	private LineRenderer lineRenderer;							// Hook Chain's LineRenderer
	private bool shouldKeepPosition;							// Whether or not keep hook leader position
	private Vector3 keepPosition;								// The position that hook should keep


	void StartTakeBackHook ()
	{
		// pull back hook
		hookStatus = HookStatus.takeback;
	}

	void HookSomething (Vector3 hookContactPoint)
	{
		// hook something
		if (nodes.Count >= minUpdateOrderNum) {
			keepPosition = hookContactPoint;
			hookStatus = HookStatus.reverseInit;
		}
	}
		
	void HookLogic ()
	{
		if (!shouldKeepPosition) {
			// keep hook leader follow owner's position
			mTransform.position = hookStartTransform.position;
		}
		
		// Pull back hook owner
		if (hookStatus == HookStatus.reverseInit) {
			shouldKeepPosition = true;
			// set leader position to hook contact point
			mTransform.position = keepPosition; 
			// set leader rotation to hook contact point
			//mTransform.rotation = Quaternion.LookRotation (hook.transform.position - keepPosition, Vector3.up);
			// reverse hook nodes
			nodes.Reverse ();
			// start reverse hook
			hookStatus = HookStatus.reverse;
			// Owner can not be controlled
			ownerController.Control(false);
		}
		
		
		// Hook shooting
		if (hookStatus == HookStatus.shooting) {
			if (nodeCount < maxNodes) {
				if(extendInterval > 0){
					if (Time.time - extendTime > extendInterval) {
						extendTime = Time.time;
						addHookNode (shootHookSpeed);
						nodeCount += shootHookSpeed;
					}
				}else{
					addHookNode (shootHookSpeed);
					nodeCount += shootHookSpeed;
				}
			} else {
				hookStatus = HookStatus.takeback;
			}
		}
		

		// Adjust hook nodes transform when owner position change
		if (hookStatus != HookStatus.reverse && nodes.Count >= minUpdateOrderNum) {
			float angle = Quaternion.Angle (hookStartTransform.rotation, mTransform.rotation);
			if (ownerController.IsMoving () && angle < updateOrderAngle) {
				bool updateNodeOrder = false;
				if (updateOrderInterval > 0) {
					if (Time.time - updateOrderTime > updateOrderInterval) {
						updateOrderTime = Time.time;
						updateNodeOrder = true;
					}
				} else {
					updateNodeOrder = true;
				}
				if (updateNodeOrder) {
					takeBackHook (updateOrderSpeed);
					addHookNode (updateOrderSpeed);			
				}
			}
		}
		
			
		// Pull back hook or reverse pull back hook.
		if (hookStatus == HookStatus.takeback || hookStatus == HookStatus.reverse) {
			
			if (nodes.Count > 0) {

				int speed = takeBackHookSpeed;
				if (takeBackInterval > 0) {
					if (Time.time - takeBackTime > takeBackInterval) {
						takeBackTime = Time.time;
						takeBackHook (speed);
					}
				} else {
					takeBackHook (speed);
				}
				
				if (hookStatus == HookStatus.reverse && hook != null) {//pull back hook owner
					FollowPrev (hook.transform, ownerTransform);
				}
			}
			
			if (nodes.Count <= 0) {
				Destroy (hook);
				Destroy (gameObject);
			}
		}
		
	}
	
	private void addHookNode (int speed)
	{
		// Bond new node
		for (int i=0; i<speed; i++) {

			Transform preTransform = LastNode ();

			Vector3 position = nextPosition(preTransform);
			Quaternion rotation = nextRotation(preTransform, position);
			GameObject hookNodeClone = Instantiate (hookNodePrefab, position, rotation) as GameObject;
			HookNode node = hookNodeClone.GetComponent<HookNode> ();
			node.hookEventListener.StartTakeBackHook += StartTakeBackHook;
//			node.canReflect = canReflect;

			Physics.IgnoreCollision (hookNodeClone.GetComponent<Collider>(), ownerTransform.gameObject.GetComponent<Collider>());
			if(nodes.Count < maxNodes){
				nodes.Add (hookNodeClone);		
			}
		}
	}
	
	private void takeBackHook (int speed)
	{
		// Remove node for take back hook chain
		for (int i=0; i<speed; i++) {
			if (nodes.Count > 0) {
				HookNode node = nodes [0].GetComponent<HookNode> ();
				node.RemoveMe ();
				nodes.RemoveAt (0);	
				if (nodes.Count == 0) {
					break;
				}
			} 
		}
	}

	
}