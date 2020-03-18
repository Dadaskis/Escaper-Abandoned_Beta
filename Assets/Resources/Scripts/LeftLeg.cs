using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftLeg : MonoBehaviour {
	private Color gizmosColor = new Color(255, 0, 255, 10);

	void OnDrawGizmos() {
		Gizmos.color = gizmosColor;
		Gizmos.DrawWireSphere (transform.position, 0.1f);
	}
}
