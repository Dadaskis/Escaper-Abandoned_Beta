using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollimatorSight : MonoBehaviour {

	public GameObject mark;
	public Weapon weapon;

	public void UpdatePosition() {
		if (weapon.shootPos != null) {
			if (transform.root.GetComponentInChildren<Player> () != null) {
				mark.SetActive (true);
				Vector3 position = weapon.shootPos.transform.position;
				Vector3 forward = weapon.shootPos.transform.forward;
				float scale = 10.0f;
				float distanceMultiplier = 1.0f;
				mark.transform.position = position + (forward * (scale * distanceMultiplier));
				mark.transform.localScale = new Vector3 (scale, scale, scale);
			} else {
				mark.SetActive (false);
			}
		}
	}

}
