using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponPartSaveData {
	public string name;
	public int id;
	public List<WeaponPartConnectionSaveData> connections = null;
}

public class WeaponPart : MonoBehaviour {
	public string name;
	public int id;
	public Weapon weapon;

	public float downRecoilMultiplier = 1.0f;
	public float upRecoilMultiplier = 1.0f;
	public float leftRecoilMultiplier = 1.0f;
	public float rightRecoilMultiplier = 1.0f;
	public float minimalRecoilPushBackPowerMultiplier = 1.0f;
	public float maximalRecoilPushBackPowerMultiplier = 1.0f;
	public float fireRateMultiplier = 1.0f;
	public float damageMultiplier = 1.0f;

	void Start() {
		WeaponData data = weapon.data;
		data.damage = (int)(data.damage * damageMultiplier);
		data.downRecoil *= downRecoilMultiplier;
		data.fireRate *= fireRateMultiplier;
		data.leftRecoil *= leftRecoilMultiplier;
		data.maximalRecoilPushBackPower *= maximalRecoilPushBackPowerMultiplier;
		data.minimalRecoilPushBackPower *= minimalRecoilPushBackPowerMultiplier;
		data.rightRecoil *= rightRecoilMultiplier;
		data.upRecoil *= upRecoilMultiplier;
	}

	public WeaponPartSaveData Save(){
		WeaponPartSaveData data = new WeaponPartSaveData ();

		data.name = name;
		data.id = id;

		List<WeaponPartConnection> connections = new List<WeaponPartConnection>();
		for (int index = 0; index < transform.childCount; index++) {
			WeaponPartConnection connection = transform.GetChild (index).GetComponent<WeaponPartConnection> ();
			if (connection != null) {
				connections.Add (connection);
			}
		}
		if (connections.Count > 0) {
			data.connections = new List<WeaponPartConnectionSaveData> ();
			foreach (WeaponPartConnection connection in connections) {
				data.connections.Add (connection.Save ());
			}
		}

		return data;
	}

	public void FillUp() {
		for (int index = 0; index < transform.childCount; index++) {
			GameObject child = transform.GetChild (index).gameObject;
			WeaponPartConnection[] childConnections = child.GetComponents<WeaponPartConnection> ();

			foreach (WeaponPartConnection connection in childConnections) {
				connection.FillUp (ref weapon);
			}
				
			LeftHand[] leftHands = GetComponentsInChildren<LeftHand> ();
			if (leftHands != null && leftHands.Length > 0) {
				weapon.leftHand = leftHands[leftHands.Length - 1];
			}
				
			RightHand[] rightHands = GetComponentsInChildren<RightHand> ();
			if (rightHands != null && rightHands.Length > 0) {
				weapon.rightHand = rightHands[rightHands.Length - 1];
			}

			ShootPos shootPos = GetComponentInChildren<ShootPos> ();
			if (shootPos != null) {
				weapon.shootPos = shootPos;
			}

			WeaponMag mag = GetComponentInChildren<WeaponMag> ();
			if (mag != null) {
				weapon.mag = mag;
			}

			WeaponBolt bolt = GetComponentInChildren<WeaponBolt> ();
			if (bolt != null) {
				weapon.bolt = bolt;
			}

			WeaponSightPosition sight = GetComponentInChildren<WeaponSightPosition> ();
			if (sight != null) {
				weapon.sight = sight;
			}

			View view = GetComponentInParent<View> ();
			if (view != null) {
				weapon.view = view;
			}

			WeaponCollimatorSight redDot = GetComponentInChildren<WeaponCollimatorSight> ();
			if (redDot != null) {
				redDot.weapon = weapon;
				if (weapon.shootPos != null) {
					redDot.UpdatePosition ();
				}
			}
		}
	}
}