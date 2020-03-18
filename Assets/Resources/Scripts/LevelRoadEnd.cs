using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoadEnd : MonoBehaviour {

	public string name = "";

	public void Start () {
		Debug.Log (name);
		LevelRoadEndManager.AddLevelRoadEnd (this);
	}

	private Color gizmosColor = new Color(255, 255, 50, 10);

	void OnDrawGizmos() {
		Gizmos.color = gizmosColor;
		Gizmos.DrawWireSphere (transform.position, 0.3f);
	}
}
