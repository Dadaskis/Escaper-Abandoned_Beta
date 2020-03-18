using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadButtonsNameFiller : MonoBehaviour {

	public List<Text> buttonsText;

	void Start () {
		int index = 0;
		foreach (Text text in buttonsText) {
			string saveName = SaveSystem.instance.saves [index].name;
			Debug.Log (saveName);
			if (saveName != null && saveName.Length > 0) {
				text.text = saveName;
			} else {
				text.text = "Save slot";
			}
			index++;
		}
	}

}
