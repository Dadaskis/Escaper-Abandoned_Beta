using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShoot : MonoBehaviour {
	public GameObject gameObject;
	private Weapon weapon;

	void Update () {
		if (Input.GetMouseButton (0) && !Player.instance.inventory.isEnabled) {
			if (Player.instance.weapon != null) {
				Player.instance.weapon.Shoot ();
			}
		}
	}
}
