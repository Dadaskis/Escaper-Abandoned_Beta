using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

	public Dictionary<string, GameObject> items = new Dictionary<string, GameObject> ();
	public static ItemManager instance;
	public string path;

	private float currentTimer = 0.0f;
	public float timerDelay = 3.0f;
	public float minDistance = 50.0f;

	void Awake () {
		instance = this;
		GameObject[] partsList = Resources.LoadAll<GameObject> (path);
		foreach(GameObject partGameObject in partsList) {
			AddItem (partGameObject);
		}
	}

	void AddItem(GameObject itemGameObject) {
		Item item = itemGameObject.GetComponent<Item> ();
		if (item != null) {
			items.Add (item.keyName, itemGameObject);
		} else {
			Debug.LogWarning ("Item havent the Item script");
		}
	}

	void Update() {
		if (currentTimer < Time.time) {
			currentTimer = Time.time + timerDelay;

			Item[] items = FindObjectsOfType<Item> ();
			foreach (Item item in items) {
				if (!item.removable) {
					continue;
				}
				if (item.gameObject.transform.parent != null) {
					continue;
				}
				if (Player.instance != null) {
					if (Vector3.Distance (Player.instance.transform.position, item.transform.position) < minDistance) {
						continue;
					}
					RaycastHit hit;
					if (Physics.Linecast (Player.instance.transform.position, item.transform.position, out hit)) {
						if (hit.collider.transform.root != item.transform.root) {
							Destroy (item.gameObject);
							continue;
						}
					}
				}
			}
		}
	}

}
