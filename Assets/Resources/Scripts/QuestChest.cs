using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class QuestChestSaveData {
	public int currentCount = 0;
	public string saveName = "";
}

public class QuestChest : MonoBehaviour	 {

	public string saveName = "";
	public string itemTag = "";
	public int maximumCount = 1;
	public int currentCount = 0;
	public Quest questToComplete;
	public TextMesh counterText;

	public class CompleteEvent : UnityEvent {}
	public CompleteEvent onCompleteEvent = new CompleteEvent();

	public QuestChestSaveData Save() {
		QuestChestSaveData saveData = new QuestChestSaveData ();
		saveData.currentCount = currentCount;
		saveData.saveName = saveName;
		Debug.Log (saveData.saveName + ": " + saveData.currentCount);
		return saveData;
	}

	public void Load(QuestChestSaveData data) {
		if (data.saveName == saveName) {
			if (currentCount < data.currentCount) {
				currentCount = data.currentCount;
				if (counterText != null) {
					counterText.text = currentCount + "/" + maximumCount;
				}
			}
		}
	}

	void Start() {
		if (counterText != null) {
			counterText.text = currentCount + "/" + maximumCount;
		}
	}

	public void Use() {
		if (currentCount >= maximumCount) {
			return;
		}
		Item item = Player.instance.inventory.FindItemWithTagInInventory (itemTag);
		if (item != null) {
			item.RemoveFromInventory ();
			currentCount++;
			if (counterText != null) {
				counterText.text = currentCount + "/" + maximumCount;
			}
			if (currentCount >= maximumCount) {
				onCompleteEvent.Invoke ();
				if (questToComplete != null) {
					QuestSystem.instance.CompleteTaggedQuest (questToComplete.tag);
				}
			}
		}
	}

}
