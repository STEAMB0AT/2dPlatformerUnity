using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DScript : MonoBehaviour {

	bool isTriggered;
	void OnTriggerEnter2D(Collider2D col) {
		Collider2D coll = col.GetComponent<Collider2D>();
		if (coll != null && coll.isTrigger == false) {
			isTriggered = true;
		}
	}
	void OnTriggerExit2D() {
		isTriggered= false;
	}
	void OnTriggerStay2D(Collider2D col) {
		Collider2D coll = col.GetComponent<Collider2D>();
		if (coll != null && coll.isTrigger == false) {
			isTriggered = true;
		}
	}

	public bool getIsTriggered(){
		return isTriggered;
	}
}
