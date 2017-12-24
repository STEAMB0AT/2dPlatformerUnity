using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS SCRIPT HANDLES THE ACTUAL CHARACTER MOVEMENT AND ANIMATION OF CURRENTLY ANIMATED PARTS
public class PhysicalMovement : MonoBehaviour {

	CharacterScript characterScript;

	GameObject[] bodyArr;
	Trigger2DScript groundCheck;
	Trigger2DScript ceilingCheck;

	Rigidbody2D rbody;
	float moveForce = 7500f;
	float jumpForce = 300f;
	float jumpTime = 200; //the number of cycles every jump lasts
	int jumpDelayCount; //the timer used for the current jump

	float currentMaxSpeed;
	float maxWalkSpeed = 150f;	
	float maxRunSpeed = 160f;	
	float maxCrouchSpeed= 60f;

	public bool grounded;
	bool idle;
	bool crouching;

	int moveMode; //used to determine what type of movement is being used (walk, jog, run, jump/fall)
	float animSpeed;

	bool aiming = false;//used to differentiate between non-animated arms and limp arms
	bool fArmAnimated = true;
	bool bArmAnimated = true;



	Vector3 fShoulderPos;// both used to reset the arms to their original position when re-animating them
	Vector3 bShoulderPos;

	public BoxCollider2D bColl1;
	public BoxCollider2D bColl2;
	// Use this for initialization
	void Start () {
		characterScript = GetComponent<CharacterScript>();
		rbody = transform.GetComponent<Rigidbody2D> ();

		ceilingCheck = transform.Find ("CeilingCheck").GetComponent<Trigger2DScript> ();
		groundCheck = transform.Find ("GroundCheck").GetComponent<Trigger2DScript> ();

		bodyArr = characterScript.getBodyArr();

		fShoulderPos = bodyArr[4].transform.localPosition;
		bShoulderPos = bodyArr[7].transform.localPosition;
	}
		
	// Update is called once per frame
	void Update () {
		
		if (jumpDelayCount >= 0) {
			jumpDelayCount -= 1;
		}
		if ((jumpDelayCount > (jumpTime - 5))&& grounded) {
			grounded = false;
		} else {
			grounded = groundCheck.getIsTriggered ();
		}
		Inputs ();
	}

	void Inputs (){//SET ALL MOVEMODE IN HERE
		//need to check if the movemode will change
		if (moveMode == 10) { //jump rising
			if (jumpDelayCount == 0 || Input.GetKeyUp (KeyCode.W)) {//end jump (either jump timer or let go of up key)
				Jump (3);
				moveMode = 11;
			} else if (Input.GetKey (KeyCode.W) && jumpDelayCount > 0) {//continue jumping
				Jump (2);
			}
		}
		if (moveMode == 11) { //ascending but not jumping any more
			if (rbody.velocity.y < 0) {//brings down quicker when falling (like smash bros)
				moveMode = 12;
			}
			if (Input.GetKeyDown (KeyCode.S)) {//brings down quicker when falling (like smash bros)
				Jump (4);
			}
		}
		if (moveMode == 12) { //controlled descending or in air
			if (Input.GetKeyDown (KeyCode.S)) {//brings down quicker when falling (like smash bros)
				Jump (4);
			}
		}
		if (!grounded && moveMode != 10 && moveMode != 11) {
			moveMode = 12;
		}

		if (grounded && moveMode != 10) {//not on ground AND not jumping
			float h = Input.GetAxis ("Horizontal");
			if ((Mathf.Abs(h) < 0.5) && (rbody.velocity.x == 0)) {//IDLE AKA not moving AND not trying to
				idle = true;
				moveMode = 0;
				animSpeed = 1;
			} else {
				idle = false;
				moveMode = 1;
				animSpeed = 1;
			}
			if (Input.GetKey (KeyCode.S)) {
				crouching = true;
				moveMode = 3;
				animSpeed = 1;
				bColl1.size = new Vector2 (19, 41);
				bColl1.offset = new Vector2 (0, -25);
				bColl2.size = new Vector2 (18, 0.5f);
				bColl2.offset = new Vector2 (0, -45.5f);
			} else {
				crouching = false;
				bColl1.size = new Vector2 (17, 66);
				bColl1.offset = new Vector2 (0, -11.75f);
				bColl2.size = new Vector2 (16, 0.5f);
				bColl2.offset = new Vector2 (0, -45.5f);
			}
			if (Input.GetKey (KeyCode.LeftShift) && (!crouching)) {
				moveMode = 2;//sprinting
				animSpeed = 1.5f;
			}
			if (Input.GetKeyDown (KeyCode.W)) {//attempt jump
				Jump (1);
				moveMode = 10;//controlled falling or dropping
			}
			if (Input.GetKeyDown (KeyCode.Q)) {//disable arms rig
				if (fArmAnimated){
					AnimateArms(3, false);
					aiming = true;
				}
				else  {
					AnimateArms(3, true);
					aiming = false;
				}
			}
			else if (Input.GetKeyDown (KeyCode.E)) {//make arms limp
				if (fArmAnimated || aiming){
					AnimateArms(3, false);
					aiming = false;
					bodyArr [4].transform.SetParent(this.transform);
					bodyArr [4].transform.GetComponent<HingeJoint2D>().connectedBody = this.rbody;
					bodyArr [4].transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Dynamic;
					bodyArr [5].transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Dynamic;
					bodyArr [6].transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Dynamic;
				}
				else {
					AnimateArms(3,true);
					aiming = false;
					bodyArr [4].transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Kinematic;
					bodyArr [5].transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Kinematic;
					bodyArr [6].transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Kinematic;
				}
				//AnimateArms(3, !FArmAnimated);simpler toggle

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

	public void AnimateArms(int armNum, bool animated){//used to change arms from animated to physical objects
		if (armNum ==1 && fArmAnimated != animated){//front arm
			bodyArr[4].GetComponent<Animator>().enabled = animated;
			fArmAnimated = animated;
			if (animated){
				bodyArr[4].transform.localPosition = fShoulderPos;
			}
		}
		else if (armNum ==2 && bArmAnimated != animated){//back arm
			bodyArr[7].GetComponent<Animator>().enabled = animated;
			bArmAnimated = animated;
			if (animated){
				bodyArr[7].transform.localPosition = bShoulderPos;
			}
		}
		else if (armNum ==3){//both arms
			AnimateArms(1, animated);
			AnimateArms(2, animated);
		}
	}

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
		bool facingRight = characterScript.getFacingRight ();
		if (!aiming){
		// If the input is moving the player right and the player is facing left...
		if (h > 0 && !facingRight)
			characterScript.Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (h < 0 && facingRight)
			characterScript.Flip();
		}
		#endregion
	}

	void FixedUpdate (){
		//NEED TO ADD ANIMATION FOR PUSHING/TRYING TO MOVE
		H_Input();

		//Setting the current maxspeed
		if (moveMode == 1) {
			currentMaxSpeed = maxWalkSpeed;
		}
		if (moveMode == 2) {
			currentMaxSpeed = maxRunSpeed;
		}
		if (moveMode == 3) {
			currentMaxSpeed = maxCrouchSpeed;
		}

		characterScript.SetAnimatorBools ("all", "Crouching", crouching);
		characterScript.SetAnimatorBools ("all", "Idle", idle);
		characterScript.SetAnimatorBools ("all", "Grounded", grounded);
		characterScript.SetAnimatorFloats ("all", "MoveSpeed", currentMaxSpeed);
		characterScript.SetAnimatorFloats ("all", "AnimSpeed", animSpeed);
		characterScript.SetAnimatorInts ("all", "MoveMode", moveMode);




	}
}
