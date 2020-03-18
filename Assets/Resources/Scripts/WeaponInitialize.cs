using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInitialize : MonoBehaviour {
	public WeaponData data;

	/*void Start () {
		Player player = Player.instance;
		GameObject weapon = Instantiate (data.weaponObject, transform);
		player.weaponData = data;
		player.WeaponObject = weapon;
		player.WeaponObject.GetComponent<Item> ().Resize ();
		InventorySystem.instance.containers [0].item = player.WeaponObject.GetComponent<Item> ();
		player.WeaponObject.GetComponent<Item> ().container = InventorySystem.instance.containers [0];
	}*/
}
