using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCompleter : MonoBehaviour {
	public Quest quest;

	void OnTriggerEnter(Collider collider) {
		if (collider.tag == "Player") {
			QuestSystem.instance.CompleteTaggedQuest (quest.tag);
		}
	}
}
