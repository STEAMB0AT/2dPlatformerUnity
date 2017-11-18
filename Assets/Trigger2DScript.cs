using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DScript : MonoBehaviour {

	bool isTriggered;
	void OnTriggerEnter2D(Collider2D col) {
		if (col.transform.tag == "Ground") {
			isTriggered = true;
		}
	}
	void OnTriggerExit2D() {
		isTriggered= false;
	}
	void OnTriggerStay2D(Collider2D col) {
		if (col.transform.tag == "Ground") {
			isTriggered = true;
		}
	}

	public bool getIsTriggered(){
		return isTriggered;
	}
}
