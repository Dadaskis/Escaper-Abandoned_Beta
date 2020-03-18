using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {
	public string keyName;

	void Start () {
		GameObject gameObject = Instantiate (ItemManager.instance.items [keyName]);
		gameObject.transform.position = transform.position;
	}
}
