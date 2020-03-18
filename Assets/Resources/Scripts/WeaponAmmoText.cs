using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAmmoText : MonoBehaviour {
	public Text text;

	void Update () {
		Player player = Player.instance;
		if (player.weapon != null) {
			text.text = player.weapon.currentAmmo + " / " + player.weapon.mag.ammoCount;
		} else {
			text.text = "";
		}
	}
}
