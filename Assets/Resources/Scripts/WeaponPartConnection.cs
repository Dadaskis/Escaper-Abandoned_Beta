using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponPartConnectionSaveData {
	public bool filled = false; 
	public WeaponPartSaveData filledPart = null;
}

public class WeaponPartConnection : MonoBehaviour {
	public List<string> partWhiteList;

	public bool filled = false;
	public WeaponPart filledPart;

	public WeaponPartConnectionSaveData Save() {
		WeaponPartConnectionSaveData data = new WeaponPartConnectionSaveData ();

		data.filled = filled;

		if (filled && filledPart != null) {
			data.filledPart = filledPart.Save ();
		}

		return data;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position, 0.1f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (transform.position, transform.position + transform.right);
	}

	public void FillUp() {
		if (!filled) {
			filled = true;
			if (partWhiteList.Count == 0) {
				return;
			}
			List<GameObject> parts;
			WeaponGeneratorManager.instance.parts.TryGetValue (
				partWhiteList [Random.Range (0, partWhiteList.Count)], out parts
			);
			if (parts == null || parts.Count == 0) {
				return;
			}
			GameObject part = Instantiate (parts [Random.Range (0, parts.Count)], transform);
			WeaponPart weaponPart = part.GetComponent<WeaponPart> ();
			if (weaponPart != null) {
				filledPart = weaponPart;
				weaponPart.FillUp ();
			} else {
				Debug.LogWarning ("Weapon part havent the WeaponPart script.");
			}
		} else {
			WeaponPart weaponPart = GetComponentInChildren<WeaponPart> ();
			if (weaponPart != null) {
				weaponPart.FillUp ();
			}
		}
	}

	public void FillUp(ref Weapon weapon) {
		if (!filled) {
			filled = true;
			if (partWhiteList.Count == 0) {
				Debug.LogWarning ("Cant fill up the weapon part connection because white list is empty.");
				return;
			}
			List<GameObject> parts;
			WeaponGeneratorManager.instance.parts.TryGetValue (
				partWhiteList [Random.Range (0, partWhiteList.Count)], out parts
			);
			if (parts == null || parts.Count == 0) {
				return;
			}
			GameObject part = Instantiate (parts [Random.Range (0, parts.Count)], transform);
			WeaponPart weaponPart = part.GetComponent<WeaponPart> ();
			if (weaponPart != null) {
				weaponPart.weapon = weapon;
				filledPart = weaponPart;
				weaponPart.FillUp ();
			} else {
				Debug.LogWarning ("Weapon part havent the WeaponPart script.");
			}
		} else {
			WeaponPart weaponPart = GetComponentInChildren<WeaponPart> ();
			if (weaponPart != null) {
				weaponPart.weapon = weapon;
				weaponPart.FillUp ();
			}
		}
	}
}