using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalMovement : MonoBehaviour {

	public CharacterScript CScript;

	GameObject[] BodyArr;
	GameObject Torso;
	GameObject Hip;
	GameObject Head;

	GameObject ArmUpperF;
	GameObject ArmLowerF;
	GameObject HandF;
	GameObject ArmUpperB;
	GameObject ArmLowerB;
	GameObject HandB;

	GameObject LegUpperF;
	GameObject LegLowerF;
	GameObject FootF;
	GameObject LegUpperB;
	GameObject LegLowerB;
	GameObject FootB;

	Trigger2DScript GroundCheck;
	Trigger2DScript CeilingCheck;

	Rigidbody2D rbody;
	float moveForce = 750f;
	float jumpForce = 30f;
	float jumpTime = 20;

	float currentMaxSpeed;
	float maxWalkSpeed = 15f;	
	float maxRunSpeed = 16f;	
	float maxCrouchSpeed= 6f;

	public bool Grounded;
	bool Idle;
	bool Crouching;

	int MoveMode; //used to determine what type of movement is being used (walk, jog, run, jump/fall)
	float AnimSpeed;

	int jumpDelayCount;

	public BoxCollider2D Bcoll1;
	public BoxCollider2D Bcoll2;
	// Use this for initialization
	void Start () {
		rbody = transform.GetComponent<Rigidbody2D> ();

		CeilingCheck = transform.Find ("CeilingCheck").GetComponent<Trigger2DScript> ();
		GroundCheck = transform.Find ("GroundCheck").GetComponent<Trigger2DScript> ();
	}
		
	// Update is called once per frame
	void Update () {
		print (rbody.velocity);

		if (jumpDelayCount >= 0) {
			jumpDelayCount -= 1;
		}
		if ((jumpDelayCount > (jumpTime - 5))&& Grounded) {
			Grounded = false;
		} else {
			Grounded = GroundCheck.getIsTriggered ();
		}
		Inputs ();
	}

	void Inputs (){//SET ALL MOVEMODE IN HERE
		//need to check if the movemode will change
		if (MoveMode == 10) { //jump rising
			if (jumpDelayCount == 0 || Input.GetKeyUp (KeyCode.W)) {//end jump (either jump timer or let go of up key)
				Jump (3);
				MoveMode = 11;
			} else if (Input.GetKey (KeyCode.W) && jumpDelayCount > 0) {//continue jumping
				Jump (2);
			}
		}
		if (MoveMode == 11) { //ascending but not jumping any more
			if (rbody.velocity.y < 0) {//brings down quicker when falling (like smash bros)
				MoveMode = 12;
			}
			if (Input.GetKeyDown (KeyCode.S)) {//brings down quicker when falling (like smash bros)
				Jump (4);
			}
		}
		if (MoveMode == 12) { //controlled descending or in air
			if (Input.GetKeyDown (KeyCode.S)) {//brings down quicker when falling (like smash bros)
				Jump (4);
			}
		}
		if (!Grounded && MoveMode != 10 && MoveMode != 11) {
			MoveMode = 12;
		}

		if (Grounded && MoveMode != 10) {//not on ground AND not jumping
			float h = Input.GetAxis ("Horizontal");
			if ((Mathf.Abs(h) < 0.5) && (rbody.velocity.x == 0)) {//IDLE AKA not moving AND not trying to
				Idle = true;
				MoveMode = 0;
				AnimSpeed = 1;
			} else {
				Idle = false;
				MoveMode = 1;
				AnimSpeed = 1;
			}
			if (Input.GetKey (KeyCode.S)) {
				Crouching = true;
				MoveMode = 3;
				AnimSpeed = 1;
				Bcoll1.size = new Vector2 (19, 41);
				Bcoll1.offset = new Vector2 (0, -25);
				Bcoll2.size = new Vector2 (18, 0.5f);
				Bcoll2.offset = new Vector2 (0, -45.5f);
			} else {
				Crouching = false;
				Bcoll1.size = new Vector2 (17, 66);
				Bcoll1.offset = new Vector2 (0, -11.75f);
				Bcoll2.size = new Vector2 (16, 0.5f);
				Bcoll2.offset = new Vector2 (0, -45.5f);
			}
			if (Input.GetKey (KeyCode.LeftShift) && (!Crouching)) {
				MoveMode = 2;//sprinting
				AnimSpeed = 1.5f;
			}
			if (Input.GetKeyDown (KeyCode.W)) {//attempt jump
				Jump (1);
				MoveMode = 10;//controlled falling or dropping
			}
		}

	}
	#region jump
	void Jump (int i){
		if (i == 1) {//start
			rbody.velocity = new Vector2 (rbody.velocity.x, jumpForce);
			//rbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
			//Debug.Log ("1");
			jumpDelayCount = (int)jumpTime;
		}
		if (i == 2) {//continue jumping
			//rbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
			rbody.velocity = new Vector2 (rbody.velocity.x, jumpForce);
			//Debug.Log ("2");

		}
		if (i == 3) {//end jump
			rbody.velocity = new Vector2 (rbody.velocity.x, rbody.velocity.y/5);
			jumpDelayCount = 0;
			//Debug.Log ("4");
		}
		if (i == 4) {//drop down "smash bros" style
			rbody.velocity = new Vector2 (rbody.velocity.x, -jumpForce);
			jumpDelayCount = 0;
			//Debug.Log ("3");


		}
	}
	#endregion

	void H_Input (){
		float h = Input.GetAxis("Horizontal");

		#region running
		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rbody.velocity.x < currentMaxSpeed)
			// ... add a force to the player.
			rbody.AddForce(Vector2.right * h * moveForce);
		
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rbody.velocity.x) > currentMaxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rbody.velocity = new Vector2(Mathf.Sign(rbody.velocity.x) * currentMaxSpeed, rbody.velocity.y);
		#endregion
		#region flip
		bool facingRight = CScript.getFacingRight ();
		// If the input is moving the player right and the player is facing left...
		if (h > 0 && !facingRight)
			CScript.Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (h < 0 && facingRight)
			CScript.Flip();
		#endregion
	}

	void FixedUpdate (){
		//NEED TO ADD ANIMATION FOR PUSHING/TRYING TO MOVE
		H_Input();

		//Setting the current maxspeed
		if (MoveMode == 1) {
			currentMaxSpeed = maxWalkSpeed;
		}
		if (MoveMode == 2) {
			currentMaxSpeed = maxRunSpeed;
		}
		if (MoveMode == 3) {
			currentMaxSpeed = maxCrouchSpeed;
		}


		CScript.SetAnimatorBools ("all", "Crouching", Crouching);
		CScript.SetAnimatorBools ("all", "Idle", Idle);
		CScript.SetAnimatorBools ("all", "Grounded", Grounded);
		CScript.SetAnimatorFloats ("all", "MoveSpeed", currentMaxSpeed);
		CScript.SetAnimatorFloats ("all", "AnimSpeed", AnimSpeed);
		CScript.SetAnimatorInts ("all", "MoveMode", MoveMode);




	}
}
