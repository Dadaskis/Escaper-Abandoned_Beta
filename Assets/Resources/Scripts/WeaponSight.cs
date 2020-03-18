using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSight : MonoBehaviour {
	
	void Update () {
		Player player = Player.instance;
		if (player.WeaponObject == null) {
			return;
		}

		player.weapon.inSight = Input.GetMouseButton (1);

		if (Input.GetKeyDown (KeyCode.R)) {
			player.weapon.Reload ();
		}
	}

}
