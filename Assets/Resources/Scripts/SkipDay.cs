using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipDay : MonoBehaviour {

	public string sceneName = "";
	public GameObject uiStory;
	public GameObject spawn;
	public float timer = 0.0f;

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
			if (timer > 20.0f) {
				if (QuestSystem.instance.quests.Count <= 0) {
					worked = true;
					SceneManager.LoadScene (sceneName);
					GraphicsSettings.CheckLights ();
					Instantiate (uiStory, Player.instance.uiCanvas.transform);
					Instantiate (spawn);
				}
			}
		}
	}

}
