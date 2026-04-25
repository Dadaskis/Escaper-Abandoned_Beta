using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestGiverSaveData {
	public bool questAdded = false;
}

public class QuestGiver : MonoBehaviour {

	public Quest quest;
	public bool questAdded = false;

	public void Start() {
		Debug.Log ("[QuestGiver] Start!");
	}

	public QuestGiverSaveData Save() {
		QuestGiverSaveData data = new QuestGiverSaveData ();
		data.questAdded = questAdded;
		return data;
	}

	public void Load(QuestGiverSaveData data) {
		data.questAdded = questAdded;
	}

	void OnTriggerStay(Collider collider) {
		if (questAdded == true) {
			return;
		}
		if (collider.tag == "Player") {
			Debug.Log ("[QuestGiver] Adding a quest: " + quest.displayName);
			QuestSystem.instance.AddQuest (quest.Instance());
			questAdded = true;
			SaveSystem.instance.AutoSave ();
		}
	}

}
