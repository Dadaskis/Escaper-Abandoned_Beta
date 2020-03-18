using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorUse : Usable {

	public int armor = 100;

	public override void Use (Item item)
	{
		if (Player.instance.character.Armor < armor) {
			Player.instance.character.Armor = armor;
			item.RemoveFromInventory ();
		}
	}
}
