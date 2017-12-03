using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	bool loaded = true;
	bool isReloading = false;
	int clipSize = 8;
	int clip = 8;
	float reloadSpeed = 2;
	float shotDelay = 0.25f;
	Animation gunAnimation;
	float holdDistance = 0.4f;
	GameObject rotatePoint;//used to accurately aim the gun's barrel and fire
	GameObject spawnPoint;//used to spawn the projectile
	public AudioSource reloadAudio;
	public AudioSource fireAudio;
	bool oneHanded;//automatically decided
	GameObject handle1;
	GameObject handle2;

	public GameObject Projectile;
	float shotPower = 100;

	//string gunType = "semi";// "auto" "bolt"

	// Use this for initialization
	void Start () {
		rotatePoint = transform.Find ("RotatePoint").gameObject;
		spawnPoint = transform.Find ("SpawnPoint").gameObject;
		handle1 = transform.Find ("Handle1").gameObject;
		gunAnimation = GetComponent<Animation> ();
		if (transform.Find ("Handle2")){
			handle2 = transform.Find ("Handle2").gameObject;
			oneHanded = false;
		}
		else {
			oneHanded = true;
		}
		clip = clipSize;
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log (clip +"\n" + loaded);
	}

	public void Fire (){
		if ((clip > 0) && (loaded == true)) {
			clip -= 1;
			//fire code here
			gunAnimation.Play("fire");
			fireAudio.Play ();
			Vector2 dir = spawnPoint.transform.position - rotatePoint.transform.position;
			dir = dir.normalized;
			float angle =  Mathf.Atan2 (spawnPoint.transform.position.y - rotatePoint.transform.position.y, spawnPoint.transform.position.x - rotatePoint.transform.position.x) * Mathf.Rad2Deg;
			GameObject bulletInstance = Instantiate (Projectile, spawnPoint.transform.position, Quaternion.Euler (0, 0, angle - 90));
			bulletInstance.GetComponent<Rigidbody2D> ().AddForce (dir*shotPower);
			if (clip < 1){
				StartCoroutine("Delay",shotDelay);
			}
		}
		else if (clip == 0) {
			Reload ();
		}
	}
	IEnumerator Delay(float delay) {//used for the delay between shots 
		loaded = false;
		yield return new WaitForSeconds (delay);
		loaded = true;
	}
	IEnumerator reload () {
		isReloading = true;
		clip = 0;
		yield return new WaitForSeconds (reloadSpeed);
		clip = clipSize;
		reloadAudio.Play();
		isReloading = false;
	}
	public void Reload (){
		if (isReloading == false) {
			StartCoroutine ("reload");
		}
	}
}
