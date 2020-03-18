using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGeneratorManager : MonoBehaviour {
	
	public Dictionary<string, List<GameObject>> parts = new Dictionary<string, List<GameObject>>();
	public static WeaponGeneratorManager instance;
	public string path;

	void Awake () {
		instance = this;
		GameObject[] partsList = Resources.LoadAll<GameObject> (path);
		Debug.Log (partsList.Length);
		foreach(GameObject partGameObject in partsList) {
			AddWeaponPart (partGameObject);
		}
	}

	void AddWeaponPart(GameObject partGameObject) {
		WeaponPart part = partGameObject.GetComponent<WeaponPart> ();
		if (part != null) {
			List<GameObject> list;
			if (!parts.TryGetValue(part.name, out list)) {
				parts.Add(part.name, new List<GameObject> ());
			}
			parts.TryGetValue (part.name, out list);
			list.Add (partGameObject);
			part.id = list.Count - 1;
		} else {
			Debug.LogWarning ("Weapon part havent the WeaponPart script");
		}
	}

}
