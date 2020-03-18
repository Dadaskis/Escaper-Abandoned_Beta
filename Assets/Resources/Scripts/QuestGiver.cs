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

	public QuestGiverSaveData Save() {
		QuestGiverSaveData data = new QuestGiverSaveData ();
		data.questAdded = questAdded;
		return data;
	}

	public void Load(QuestGiverSaveData data) {
		data.questAdded = questAdded;
	}

	void OnTriggerEnter(Collider collider) {
		if (questAdded == true) {
			return;
		}
		if (collider.tag == "Player") {
			QuestSystem.instance.AddQuest (quest.Instance());
			questAdded = true;
			SaveSystem.instance.AutoSave ();
		}
	}

}
