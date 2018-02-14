using UnityEngine;
using System.Collections;

public class PlayerMovement : BaseCharacterController
{
	// shooting state
	private bool shooting;

	// if player can be controlled
	private bool canControl = true;	

	// A smoothing value for turning the player.
	public float turnSmoothing = 15f;

	// The damping for the speed parameter
	public float speedDampTime = 0.1f;
	
	// The max speed of player
	public float speedMax = 5.5f;
	// current used max speed
	private float useSpeedMax;

	// Reference to the animator component.
	private Animator anim;
	
	// invalid position
	private Vector3 invalidPosition = new Vector3 (-999, -999, -999);
	// where player wanna go 
	private Vector3 targetPosition;

	void Awake ()
	{
			// Setting up the references.
			anim = GetComponent<Animator> ();

	}

	void Start ()
	{
			// set current max speed to default max speed
			useSpeedMax = speedMax;
			// in the beginning, nowhere player wanna go 
			targetPosition = invalidPosition;
	}

	void FixedUpdate ()
	{
			if (canControl) {
					// Cache the inputs.
					float h = 0.0f;
					float v = 0.0f;
					#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
					h = joystickLeft.position.x;
					v = joystickLeft.position.y;
					#else
					h = Input.GetAxis ("Horizontal");
					v = Input.GetAxis ("Vertical");
					#endif
		
					bool sneak = false;//Input.GetButton ("Sneak");
					bool shoot = false;
					if (shooting) {
							shoot = true;
							shooting = false;
					}
		
					MovementManagement (h, v, sneak, shoot);
			} else {
					targetPosition = invalidPosition;
					MovementManagement (0f, 0f, false, false);
			}
	
	}

	void MovementManagement (float horizontal, float vertical, bool sneaking, bool shooting)
	{

			// If there is some axis input...
			if (horizontal != 0f || vertical != 0f) {
					// ... set the players rotation and set the speed parameter to max speed.
					Rotating (new Vector3 (horizontal, 0f, vertical));
					targetPosition = invalidPosition;

			} else if (targetPosition != invalidPosition) {//go to the target position

					Vector3	targetDirection = targetPosition - transform.position;
					targetDirection.y = 0;
					float dist = targetDirection.magnitude;
					targetDirection = targetDirection.normalized;
					if (dist < 0.1f) {
							//arrive at the destination
							targetPosition = invalidPosition;
					} else if (dist < 1.0f) {
							//walk
							Rotating (targetDirection);
					} else {
							//run
							Rotating (targetDirection);
							//anim.SetFloat (hash.speedFloat, useSpeedMax, speedDampTime, Time.deltaTime);
					}

			} else {
					// Otherwise set the speed parameter to 0.
			}
	}

	void Rotating (Vector3 targetDirection)
	{
			// Create a new vector of the horizontal and vertical inputs.
			//Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);

			// Create a rotation based on this new vector assuming that up is the global y axis.
			Quaternion targetRotation = Quaternion.LookRotation (targetDirection, Vector3.up);

			// Create a rotation that is an increment closer to the target rotation from the player's rotation.
			Quaternion newRotation = Quaternion.Lerp (GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);

			// Change the players rotation to this new rotation.
			GetComponent<Rigidbody>().MoveRotation (newRotation);
	}


	public override void Control (bool control)
	{
		// set player Whether can be control
		canControl = control;
	}

	public override bool IsMoving ()
	{
		// check if player is moving
		return IsState (0, 0);
	}

	public override bool IsShooting ()
	{
		// check if player is shooting
		return shooting;
	}

	private bool IsState (int layer, int state)
	{
		// check play's state by animator state
		return anim.GetCurrentAnimatorStateInfo (layer).nameHash == state;
	}

	private bool IsStateByTag (int layer, float tag)
	{
		// check play's state by animator tag
		return anim.GetCurrentAnimatorStateInfo (layer).tagHash == tag;
	}

	public override void SlowDownMovingSpeed ()
	{
		// slow down play moving speed
		useSpeedMax = 2;
	}

	public override void NormalMovingSpeed ()
	{
		// set current max speed value
		useSpeedMax = speedMax;
	}

	public override void Shoot ()
	{
		// shoot
		shooting = true;
	}

	public override void MoveTo (Vector3 target)
	{
		//set where player wanna go 
		targetPosition = target;
	}
}
