using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//THIS SCRIPT HANDLES THE AIMING AND ARMS/GUN MOVEMENT as well as items
public class ItemHandler : MonoBehaviour {
	CharacterScript characterScript;
	PhysicalMovement physicalMovementScript;

	GameObject equippedObject;
	GameObject[] bodyArr;
	int charge;

	Vector2 mousePos;
	// Use this for initialization
	void Start () {
		characterScript = GetComponent<CharacterScript>();
		physicalMovementScript = GetComponent<PhysicalMovement>();

		bodyArr = characterScript.getBodyArr();
	}
	
	// Update is called once per frame
	void Update () {
		mousePos = characterScript.getMousePos();
		equippedObject = characterScript.GetEquippedObject();

	}
	public void Charge (int i) {//used by the main script to charge up and throw an item
		if (i == 1){	//start charge
			charge = 20;
			physicalMovementScript.AnimateArms(1, false);
			AimArms(bodyArr[4], 0);
		}
		if (i == 2){	//keep charging
			if (charge < 100) {
				charge += 1;
			}
			AimArms(bodyArr[4], 120);
		}
		if (i == 3){	//end charge
			physicalMovementScript.AnimateArms(1, true);
			AimArms(bodyArr[4], 0);
			Throw(charge);
			charge = 0;
		}
	}
	Vector2 GetMousePosToEyes(int distanceFromEyes){//if called with argument "0", 
		Vector3 centerofEyes = bodyArr[1].transform.position;
		Vector2 mousePosToEyes = new Vector3 (mousePos.x - centerofEyes.x, mousePos.y - centerofEyes.y);
		if (distanceFromEyes != 0)
		{
			Ray2D aimRay = new Ray2D (centerofEyes, mousePosToEyes);
			return (aimRay.GetPoint (distanceFromEyes));
		}
		else return mousePosToEyes;

	}
	void Throw (int power){
		Vector3 targetVector = GetMousePosToEyes(0);
		equippedObject.transform.parent = null;
		equippedObject.transform.position = GetMousePosToEyes(5);
		equippedObject.GetComponent<Rigidbody2D>().velocity = (Vector3.Normalize(targetVector) * power);

		characterScript.UnequipObject(equippedObject);
	}
	void AimArms (GameObject part, int offset){//arms for now, eventually want to be able to aim the head for looking and laser eyes
		Vector3 centerofEyes = bodyArr[1].transform.position;
		Vector2 mousePosToEyes = new Vector3 (mousePos.x - centerofEyes.x, mousePos.y - centerofEyes.y);
		Ray2D aimRay = new Ray2D (centerofEyes, mousePosToEyes);
		Vector2 aimPos = aimRay.GetPoint (0);
		Vector2 targetPos = aimRay.GetPoint (1 * 2);//used to angle the gun
		float angleGun = Mathf.Atan2 (targetPos.y - aimPos.y, targetPos.x - aimPos.x) * Mathf.Rad2Deg;
		part.transform.position = new Vector3 (aimPos.x, aimPos.y, 0); //+ (pistol.transform.position - GUNSCRIPT.RotatePoint.transform.position);
		part.transform.rotation = Quaternion.Euler (0, 0, 90+ angleGun + offset);
	}
}
