using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTextUpdate : MonoBehaviour {

	public Text text;

	private float currentTimer = 0.0f;
	public float delay = 1.0f;

	void Start () {
		QuestSystem.instance.onNewQuest.AddListener (OnNewQuest);
		QuestSystem.instance.onCompletedQuest.AddListener (OnCompletedQuest);
	}

	void FixedUpdate() {
		if (currentTimer < Time.time) {
			currentTimer = Time.time + delay;
			text.text = "";
		}
	}

	void OnNewQuest(Quest quest) {
		string currentText = text.text;
		currentText += "\nNew objective: " + quest.displayName;
		text.text = currentText;
		currentTimer = Time.time + delay;
	}

	void OnCompletedQuest(Quest quest) {
		string currentText = text.text;
		currentText += "\nCompleted objective: " + quest.displayName;
		text.text = currentText;
		currentTimer = Time.time + delay;
	}

}
