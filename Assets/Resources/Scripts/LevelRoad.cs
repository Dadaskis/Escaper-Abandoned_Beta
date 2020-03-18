using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRoad : MonoBehaviour {

	public string sceneName = "";
	public string roadEndName = "";
	public bool makeAutosave = true;
	public bool destroyPlayer = false;

	/*IEnumerator LoadLevel() {
		LevelRoadEndManager.SetRequestPath (roadEndName);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync (sceneName);
		asyncOperation.allowSceneActivation = true;
		while (asyncOperation.progress < 0.9f) {
			yield return null;
		}
	}*/

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.GetComponent<Player> () != null) {
			//StartCoroutine (LoadLevel ());
			if (makeAutosave) {
				SaveSystem.instance.AutoSave ();
			}
			if (destroyPlayer) {
				Destroy (Player.instance.gameObject);
			}
			LevelRoadEndManager.SetRequestPath(roadEndName, makeAutosave, destroyPlayer);
			SceneManager.LoadScene(sceneName);
		}
	}
}
