using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestSaveData {
	public bool completed = false;
	public string tag = "";
	public string displayName = "";
}

[CreateAssetMenu(menuName = "New quest", fileName = "Create new quest")]
public class Quest : ScriptableObject {
	public bool completed = false;
	public string tag = "";
	public string displayName = "";

	public void Complete() {
		completed = true;
	}

	public Quest Instance() {
		Quest quest = ScriptableObject.CreateInstance<Quest> ();
		quest.tag = tag;
		quest.displayName = displayName;
		quest.completed = completed;
		return quest;
	}

	public QuestSaveData Save() {
		QuestSaveData data = new QuestSaveData ();
		data.completed = completed;
		data.displayName = displayName;
		data.tag = tag;
		return data;
	}

	public void Load(QuestSaveData data) {
		completed = data.completed;
		displayName = data.displayName;
		tag = data.tag;
	}
}
