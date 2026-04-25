using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipDay : MonoBehaviour {

	public string sceneName = "";
	public GameObject uiStory;
	public GameObject spawn;
	public float timer = 0.0f;
	public List<Quest> quests;
	

	private bool worked = false;

	void Start() {
		GraphicsSettings.CheckLights ();
	}

	void Update() {
		timer += Time.deltaTime;
	}

	void OnTriggerStay(Collider collider) {
		if (collider.gameObject.GetComponent<Player> () == null) {
			return;
		}
		if (!worked) {
			if (timer > 2.0f) {
				if (QuestSystem.instance.quests.Count <= 0) {
					worked = true;
					SceneManager.LoadScene (sceneName);
					GraphicsSettings.CheckLights ();
					Instantiate (uiStory, Player.instance.uiCanvas.transform);
					GameObject spawnObj = Instantiate (spawn);
					Debug.Log (spawnObj.transform.position);

					foreach (Quest quest in quests) {
						Debug.Log ("[FakeQuestGiver] Adding a quest: " + quest.displayName);
						QuestSystem.instance.AddQuest (quest.Instance ());
						SaveSystem.instance.AutoSave ();
					}

					Debug.Log ("Nullifying chests in SkipDay too");
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
		}
	}

}
