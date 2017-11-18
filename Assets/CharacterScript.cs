using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour {

	public PhysicalMovement PMovementScript;

	public GameObject Torso;
	GameObject Hip;
	GameObject Head;
	GameObject Neck;

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

	GameObject[] BodyArr;

	Animator TorsoAnimator;
	Animator NeckAnimator;
	Animator ArmFAnimator;
	Animator ArmBAnimator;
	Animator HipAnimator;

	public BoxCollider2D coll1;
	public BoxCollider2D coll2;


	public Camera cam;
	public bool facingRight = true;	

	public GameObject[] getBodyArr(){
		return BodyArr;
	}
	// Use this for initialization
	void Start () {
		
		GetBody ();

		TorsoAnimator = Torso.GetComponent<Animator> ();
		NeckAnimator = Neck.GetComponent<Animator> ();
		ArmFAnimator = ArmUpperF.GetComponent<Animator> ();
		ArmBAnimator = ArmUpperB.GetComponent<Animator> ();
		HipAnimator = Hip.GetComponent<Animator> ();
	}

	void GetBody (){
		
		Neck = Torso.transform.Find("Neck").gameObject;
		Head = Neck.transform.Find("Head").gameObject;
		Hip = Torso.transform.Find("Hip").gameObject;

		ArmUpperF = Torso.transform.Find("ArmUpperF").gameObject;
		ArmLowerF = ArmUpperF.transform.Find("ArmLowerF").gameObject;
		HandF = ArmLowerF.transform.Find("HandF").gameObject;
		ArmUpperB = Torso.transform.Find("ArmUpperB").gameObject;
		ArmLowerB = ArmUpperB.transform.Find("ArmLowerB").gameObject;
		HandB = ArmLowerB.transform.Find("HandB").gameObject;

		LegUpperF = Hip.transform.Find("LegUpperF").gameObject;
		LegLowerF = LegUpperF.transform.Find("LegLowerF").gameObject;
		FootF = LegLowerF.transform.Find ("FootF").gameObject;
		LegUpperB = Hip.transform.Find("LegUpperB").gameObject;
		LegLowerB = LegUpperB.transform.Find("LegLowerB").gameObject;
		FootB = LegLowerB.transform.Find("FootB").gameObject;

		BodyArr = new GameObject[]{
			Torso, Head, Neck, Hip, ArmUpperF, ArmLowerF, HandF, ArmUpperB, ArmLowerB, HandB, LegUpperF, LegLowerF, FootF, LegUpperB, LegLowerB, FootB
		};
	}

	public bool getFacingRight (){
		return facingRight;
	}
	public void Flip () {
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;	
	}
	// Update is called once per frame
	void Update () {
		// Read the input in Update so button presses aren't missed.
		cam.transform.position = new Vector3(this.transform.position.x,this.transform.position.y,cam.transform.position.z);
		if (Input.GetKeyDown ("t")) {//reset pos
			transform.position = new Vector3 (0, 5, 0);
		}
		if (Input.GetKeyDown ("r")) { //kill instantly
			
			for (int i = 0; i < BodyArr.Length; i++) {
				BodyArr [i].transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Dynamic;
			}
			TorsoAnimator.enabled = false;
			NeckAnimator.enabled = false;
			ArmFAnimator.enabled = false;
			ArmBAnimator.enabled = false;
			HipAnimator.enabled = false;
			coll1.enabled = false;
			coll2.enabled = false;

			Head.transform.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-10000,0));
			PMovementScript.enabled = false;
			this.enabled = false;

		}

	}
	public void SetRunning (float f){
		TorsoAnimator.SetFloat ("Walking", f);
	}
	public void SetAnimatorBools (string Animators, string Parameter, bool b){
		if (Animators == "all") {
			TorsoAnimator.SetBool (Parameter, b);
			NeckAnimator.SetBool (Parameter, b);
			ArmFAnimator.SetBool (Parameter, b);
			ArmBAnimator.SetBool (Parameter, b);
			HipAnimator.SetBool (Parameter, b);
		}
	}
	public void SetAnimatorInts (string Animators, string Parameter, int i){
		if (Animators == "all") {
			TorsoAnimator.SetInteger (Parameter, i);
			NeckAnimator.SetInteger (Parameter, i);
			ArmFAnimator.SetInteger (Parameter, i);
			ArmBAnimator.SetInteger (Parameter, i);
			HipAnimator.SetInteger (Parameter, i);
		}
	}
	public void SetAnimatorFloats (string Animators, string Parameter, float f){
		if (Animators == "all") {
			TorsoAnimator.SetFloat (Parameter, f);
			NeckAnimator.SetFloat (Parameter, f);
			ArmFAnimator.SetFloat (Parameter, f);
			ArmBAnimator.SetFloat (Parameter, f);
			HipAnimator.SetFloat (Parameter, f);
		} else if (Animators == "torso") {

		}
	}
}
