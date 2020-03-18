using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class QuestSystemSaveData {
	public List<QuestSaveData> quests = new List<QuestSaveData>();
}

public class QuestSystem : MonoBehaviour {

	public class QuestEvent : UnityEvent<Quest> {}
	public QuestEvent onNewQuest = new QuestEvent ();
	public QuestEvent onCompletedQuest = new QuestEvent ();

	public static QuestSystem instance;

	public List<Quest> quests = new List<Quest>();

	public QuestSystemSaveData Save() {
		QuestSystemSaveData save = new QuestSystemSaveData ();
		foreach (Quest quest in quests) {
			save.quests.Add (quest.Save ());
		}
		return save;
	}

	public void Load(QuestSystemSaveData save){
		quests = new List<Quest> ();
		foreach (QuestSaveData data in save.quests) {
			Quest quest = ScriptableObject.CreateInstance<Quest> ();
			quest.Load (data);
			quests.Add (quest);
		}
	}

	void Awake () {
		instance = this;	
	}

	public void AddQuest(Quest quest) {
		quests.Add (quest);
		onNewQuest.Invoke (quest);
	}

	public List<Quest> FindTaggedQuests(string tag){
		List<Quest> questList = new List<Quest> ();
		foreach (Quest quest in quests) {
			if (quest.tag == tag) {
				questList.Add (quest);
			}
		}
		return questList;
	}

	public Quest FindTaggedQuest(string tag) {
		Quest quest = null;
		foreach (Quest quest2 in quests) {
			if (quest2.tag == tag) {
				quest = quest2;
				break;
			}
		}
		return quest;
	}

	public Quest CompleteTaggedQuest(string tag) {
		Quest quest = FindTaggedQuest (tag);
		if (quest != null) {
			quest.Complete ();
			return quest;
		}
		return null;
	}

	void Update () {
		List<int> questsToDelete = null;

		int counter = 0;
		foreach (Quest quest in quests) {
			if (quest == null) {
				if (questsToDelete == null) {
					questsToDelete = new List<int> ();
				}
				questsToDelete.Add (counter);
				continue;
			}
			if (quest.completed) {
				onCompletedQuest.Invoke (quest);
				if (questsToDelete == null) {
					questsToDelete = new List<int> ();
				}
				questsToDelete.Add (counter);
			}
			counter++;
		}

		if (questsToDelete != null) {
			int countOfDeletedQuests = 0;
			foreach (int index in questsToDelete) {
				quests.RemoveAt (index - countOfDeletedQuests);
			}
		}
	}
}
