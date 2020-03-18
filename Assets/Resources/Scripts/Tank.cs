using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Effects;

public class Tank : MonoBehaviour {

	public QuestChest chest;
	public Transform shootPos;
	public GameObject explosionObject;
	public GameObject fireObject;
	public DestroyableObject destroyableObject;

	void Start() {
		chest.onCompleteEvent.AddListener (ShootOnComplete);
	}

	void ShootOnComplete() {
		GameObject explosion0 = Instantiate (explosionObject);
		GameObject explosion1 = Instantiate (explosionObject);
		GameObject fire = Instantiate (fireObject);

		ParticleSystemMultiplier multiplier = explosion1.GetComponent<ParticleSystemMultiplier> ();
		multiplier.multiplier = 20.0f;

		explosion0.transform.position = shootPos.position;
		explosion1.transform.position = destroyableObject.transform.position;
		fire.transform.position = destroyableObject.transform.position;

		destroyableObject.Damage (int.MaxValue - 1);
	}

}
