using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRoadEndManager : MonoBehaviour {

	public static LevelRoadEndManager instance;
	public Dictionary<string, Transform> levelRoadEnds = new Dictionary<string, Transform>(); 
	public GameObject player;
	public string requestPath = "";
	public bool makeAutosave = true;
	public bool destroyPlayer = false;
	private float timer = 0.0f;

	void Awake() {
		instance = this;
	}

	void Update() {
		if (requestPath != null && requestPath.Length > 1 && !destroyPlayer) {
			GraphicsSettings.CheckLights ();
			SaveSystem.instance.LoadQuests ();

			Vector3 spawnPosition = GetLevelRoadEndPosition(requestPath);
			if (spawnPosition != Vector3.zero) {
				Player player = Player.instance;
				player.transform.position = spawnPosition;

				// Apply rotation and reset velocity from the LevelRoadEnd
				LevelRoadEnd end = GetLevelRoadEnd(requestPath);
				if (end != null) {
					end.ApplyToPlayer(player);
				}

				requestPath = null;
				if (makeAutosave) {
					SaveSystem.instance.AutoSave ();
				}
				Scav.count = 0;
			}
		}
	}

	public static LevelRoadEnd GetLevelRoadEnd(string name) {
		Transform endTransform = null;
		if (instance.levelRoadEnds.TryGetValue(name, out endTransform)) {
			if (endTransform != null) {
				return endTransform.GetComponent<LevelRoadEnd>();
			}
		}
		return null;
	}

	public static void SetRequestPath(string path, bool makeAutosave, bool destroyPlayer){
		instance.requestPath = path;
		instance.makeAutosave = makeAutosave;
		instance.destroyPlayer = destroyPlayer;
	}

	public static void AddLevelRoadEnd(LevelRoadEnd road) {
		try{
			Transform testTransform = null;
			if (!instance.levelRoadEnds.TryGetValue (road.name, out testTransform)) {
				instance.levelRoadEnds.Add (road.name, road.transform);
			} else {
				instance.levelRoadEnds [road.name] = road.transform;
			}
		} catch(System.Exception ex) {
			return;
		}
	}

	public static Vector3 GetLevelRoadEndPosition(string name){
		Transform endTransform = null;
		if (instance.levelRoadEnds.TryGetValue (name, out endTransform)) {
			if (endTransform != null) {
				return endTransform.position + new Vector3 (0.0f, 1.0f, 0.0f);
			}
		}
		return Vector3.zero;
	}
}
