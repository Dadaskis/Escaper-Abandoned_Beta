using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestDestroyable : MonoBehaviour {

	public Quest questToComplete;
	public DestroyableObject destroyableObject;

	void Start() {
		destroyableObject.onDestroyEvent.AddListener (OnDestroy);	
	}

	void OnDestroy() {
		QuestSystem.instance.CompleteTaggedQuest (questToComplete.tag);
	}

}
