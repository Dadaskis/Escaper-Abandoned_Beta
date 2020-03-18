using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalUse : Usable {

	public int health = 100;

	public override void Use(Item item) {
		Player.instance.character.Health = health;
		item.RemoveFromInventory ();
	}

}
