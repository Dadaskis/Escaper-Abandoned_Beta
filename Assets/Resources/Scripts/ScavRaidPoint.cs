using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavRaidPoint : MonoBehaviour {

	void Start () {
		ScavLogic.Raids.Add (this.transform.position);	
	}

}
