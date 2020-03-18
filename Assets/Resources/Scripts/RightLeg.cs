using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightLeg : MonoBehaviour {
	private Color gizmosColor = new Color(255, 255, 0, 10);

	void OnDrawGizmos() {
		Gizmos.color = gizmosColor;
		Gizmos.DrawWireSphere (transform.position, 0.1f);
	}
}
