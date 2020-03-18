using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseInitialize : MonoBehaviour {

	public WeaponPart part;

	public void Start() {
		part.weapon.Initialize ();
	}

}
