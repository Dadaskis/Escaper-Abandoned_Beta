using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifyQuestChests : MonoBehaviour {
	void Start () {
		QuestChest[] chests = FindObjectsOfType<QuestChest> ();
		foreach (QuestChest chest in chests) { 
			chest.currentCount = 0;
		}
		if (SaveSystem.instance.saves [0] != null) {
			SaveSystem.instance.saves [0].questChestsSaveData = new List<QuestChestSaveData> ();
		}
		SaveSystem.instance.AutoSave ();
	}
}
