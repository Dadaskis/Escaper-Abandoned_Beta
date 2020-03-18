using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPartAutoFillUp : MonoBehaviour {

	public WeaponPartConnection part;
	public bool markAsDropped = true;
	public bool markAsBot = true;
	public bool markAsRemovable = true;

	void Start () {
		part.FillUp ();
		Weapon weapon = GetComponentInChildren<Weapon> ();
		weapon.dropped = markAsDropped;
		weapon.botCase = markAsBot;
		weapon.item.removable = markAsRemovable;
	}
}
