using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand : MonoBehaviour {
	private Color gizmosColor = new Color(255, 0, 0, 10);

	void OnDrawGizmos() {
		Gizmos.color = gizmosColor;
		Gizmos.DrawWireSphere (transform.position, 0.1f);
	}
}
