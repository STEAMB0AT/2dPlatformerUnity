using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour {

	PhysicalMovement physicalMovementScript;
	ItemHandler itemHandlerScript;

	GameObject selectedObject;
	GameObject equippedObject;

	GameObject torso;
	GameObject hip;
	GameObject head;
	GameObject neck;

	GameObject armUpperF;
	GameObject armLowerF;
	GameObject handF;
	GameObject armUpperB;
	GameObject armLowerB;
	GameObject handB;
	GameObject legUpperF;
	GameObject legLowerF;
	GameObject footF;
	GameObject legUpperB;
	GameObject legLowerB;
	GameObject footB;

	GameObject[] bodyArr;

	Animator torsoAnimator;
	Animator neckAnimator;
	Animator armFAnimator;
	Animator armBAnimator;
	Animator hipAnimator;

	Vector2 mousePos;

	public float torsoH = 1f;
	public float torsoW = 1f;


	public BoxCollider2D coll1;
	public BoxCollider2D coll2;

	public Camera cam;
	public bool facingRight = true;	

	// Use this for initialization


	void Start () {
		physicalMovementScript = GetComponent<PhysicalMovement>();
		itemHandlerScript = GetComponent<ItemHandler>();


		//setting the bodyparts and the array of them
		torso = transform.Find("Torso").gameObject;
		neck = torso.transform.Find("Neck").gameObject;
		head = neck.transform.Find("Head").gameObject;
		hip = torso.transform.Find("Hip").gameObject;
		armUpperF = torso.transform.Find("ArmUpperF").gameObject;
		armLowerF = armUpperF.transform.Find("ArmLowerF").gameObject;
		handF = armLowerF.transform.Find("HandF").gameObject;
		armUpperB = torso.transform.Find("ArmUpperB").gameObject;
		armLowerB = armUpperB.transform.Find("ArmLowerB").gameObject;
		handB = armLowerB.transform.Find("HandB").gameObject;
		legUpperF = hip.transform.Find("LegUpperF").gameObject;
		legLowerF = legUpperF.transform.Find("LegLowerF").gameObject;
		footF = legLowerF.transform.Find ("FootF").gameObject;
		legUpperB = hip.transform.Find("LegUpperB").gameObject;
		legLowerB = legUpperB.transform.Find("LegLowerB").gameObject;
		footB = legLowerB.transform.Find("FootB").gameObject;


		bodyArr = new GameObject[]{
			torso, head, neck, hip, armUpperF, armLowerF, handF, armUpperB, armLowerB, handB, legUpperF, legLowerF, footF, legUpperB, legLowerB, footB
		};

		//setting the animators
		torsoAnimator = torso.GetComponent<Animator> ();
		neckAnimator = neck.GetComponent<Animator> ();
		armFAnimator = armUpperF.GetComponent<Animator> ();
		armBAnimator = armUpperB.GetComponent<Animator> ();
		hipAnimator = hip.GetComponent<Animator> ();
	}
	public GameObject[] getBodyArr(){
		return bodyArr;
	}
	public bool getFacingRight (){
		return facingRight;
	}
	public Vector2 getMousePos (){
		return mousePos;
	}
	public void Flip () {
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;	
	}
	void ChangeHeight(){
		for (int i = 4; i < 10; i++) {
			bodyArr [i].transform.localScale = new Vector3 (torsoW,torsoH,1);
		}
	}
	// Update is called once per frame
	void Update () {
	
		ChangeHeight();
		// Read the input in Update so button presses aren't missed.
		mousePos = cam.ScreenToWorldPoint (Input.mousePosition);
		if (Input.GetKeyDown ("t")) {//reset pos
			transform.position = new Vector3 (0, 5, 0);
		}
		if (Input.GetKeyDown ("r")) { //kill instantly
			Kill();
		}
		GetHoveredObject();
		//finds if mouse is over object to equip
		if (Input.GetMouseButtonDown(0)){ // if left button pressed...
			if (selectedObject != null){
				if (selectedObject.tag == "Item"){
					EquipObject(selectedObject);
				}
			}
		}
		if (Input.GetMouseButtonDown(1)){ // if right button down
			if (equippedObject != null){
				itemHandlerScript.Charge(1);
			}
		}
		if (Input.GetMouseButton(1)){ // if right button held
			if (equippedObject != null){
				itemHandlerScript.Charge(2);
			}
		}
		if (Input.GetMouseButtonUp(1)){ // if right button released
			if (equippedObject != null){
				itemHandlerScript.Charge(3);
			}
		}
	}
	public GameObject GetEquippedObject (){
		return equippedObject;
	}
	public void EquipObject (GameObject item){
		item.transform.GetComponent <Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		if (item.transform.GetComponent<PolygonCollider2D>() != null){
			item.transform.GetComponent <PolygonCollider2D>().isTrigger = true;
		}
		else if (item.transform.GetComponent<BoxCollider2D>() != null){
			item.transform.GetComponent <BoxCollider2D>().isTrigger = true;
		}
		else if (item.transform.GetComponent<CircleCollider2D>() != null){
			item.transform.GetComponent <CircleCollider2D>().isTrigger = true;
		}
		else if (item.transform.GetComponent<CapsuleCollider2D>() != null){
			item.transform.GetComponent <CapsuleCollider2D>().isTrigger = true;
		}
		item.transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		item.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;
		item.transform.SetParent(bodyArr[6].transform);
		item.transform.localPosition = Vector3.zero;
		equippedObject = item;
	}
	public void UnequipObject (GameObject item) {
		item.transform.GetComponent <Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
		item.transform.parent = null;
		item.transform.GetComponent <Collider2D>().isTrigger = false;
		
		equippedObject = null;
	}

	private void GetHoveredObject() {
		
		//returns objects under the mouse
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D  hit;
		hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
		if (hit){
			//Debug.Log(hit.transform.name);
			selectedObject = hit.transform.gameObject;
		}
		else {
			selectedObject = null;
		}
	}
	private GameObject GetClosestObject(string tag, Vector2 pos) {
		//method that returns the closest gameobject to a position

		GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag (tag);
		GameObject closestObject = objectsWithTag[0];
		for (int i= 0; i < objectsWithTag.Length; i ++)
		{
			//compares distances and switches for the shortest distance
			if(Vector2.Distance(pos, objectsWithTag[i].transform.position) <= Vector2.Distance(pos, closestObject.transform.position))
			{
				closestObject = objectsWithTag[i];
			}
		}
		return closestObject;
	}
	private void Kill (){
		for (int i = 0; i < bodyArr.Length; i++) {
			bodyArr [i].transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Dynamic;
			bodyArr [i].transform.GetComponent<Rigidbody2D> ().simulated = true;
			bodyArr [i].transform.GetComponent<BoxCollider2D> ().isTrigger = false;
		}
		torsoAnimator.enabled = false;
		neckAnimator.enabled = false;
		armFAnimator.enabled = false;
		armBAnimator.enabled = false;
		hipAnimator.enabled = false;
		coll1.enabled = false;
		coll2.enabled = false;

		head.transform.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-10000,0));
		physicalMovementScript.enabled = false;
		this.enabled = false;
	}

	public void SetRunning (float f){
		torsoAnimator.SetFloat ("Walking", f);
	}
	public void SetAnimatorBools (string Animators, string Parameter, bool b){
		if (Animators == "all") {
			torsoAnimator.SetBool (Parameter, b);
			neckAnimator.SetBool (Parameter, b);
			armFAnimator.SetBool (Parameter, b);
			armBAnimator.SetBool (Parameter, b);
			hipAnimator.SetBool (Parameter, b);
		}
	}
	public void SetAnimatorInts (string Animators, string Parameter, int i){
		if (Animators == "all") {
			torsoAnimator.SetInteger (Parameter, i);
			neckAnimator.SetInteger (Parameter, i);
			armFAnimator.SetInteger (Parameter, i);
			armBAnimator.SetInteger (Parameter, i);
			hipAnimator.SetInteger (Parameter, i);
		}
	}
	public void SetAnimatorFloats (string Animators, string Parameter, float f){
		if (Animators == "all") {
			torsoAnimator.SetFloat (Parameter, f);
			neckAnimator.SetFloat (Parameter, f);
			armFAnimator.SetFloat (Parameter, f);
			armBAnimator.SetFloat (Parameter, f);
			hipAnimator.SetFloat (Parameter, f);
		} else if (Animators == "torso") {

		}
	}
}
