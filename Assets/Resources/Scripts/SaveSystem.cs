using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save {
	public string name;
	public string sceneName;
	public PlayerSaveData playerData;
	public int scavsCount = 0;
	public int scavsCountLimit = 0;
	public List<ScavSave> scavsData = new List<ScavSave> ();
	public List<WeaponSaveData> droppedWeaponsData = new List<WeaponSaveData> ();
	public List<ItemSaveData> droppedItemsData = new List<ItemSaveData> ();
	public QuestSystemSaveData questSystemSaveData;
	public List<QuestGiverSaveData> questGiversSaveData = new List<QuestGiverSaveData> ();
	public List<QuestChestSaveData> questChestsSaveData = new List<QuestChestSaveData> ();
	public List<DestroyableObjectSaveData> destroyableObjectsSaveData = new List<DestroyableObjectSaveData>();
}

public class SaveSystem : MonoBehaviour {

	public class WeaponDeserializationQueueData {
		public WeaponPartSaveData partData;
		public Transform transform;
	}

	public static SaveSystem instance;

	public bool saved = false;
	public GameObject emptyScav;
	public GameObject playerSpawnObject;

	public List<Save> saves = new List<Save>();
	private Save save = null;
	private int countOfSaves = 12;

	private bool isLoading = false;

	void Serialize() {
		for (int index = 0; index < countOfSaves; index++) {
			string json = JsonUtility.ToJson(saves[index], true);
			System.IO.File.WriteAllText ("warfare" + index + ".save", json);
		}
	}

	void Deserialize() {
		for (int index = 0; index < countOfSaves; index++) {
			string json = System.IO.File.ReadAllText ("warfare" + index + ".save");
			Debug.Log (json);
			if (json.Length > 2) {
				saves[index] = JsonUtility.FromJson<Save>(json);
			}
		}
	}

	void Awake() {
		DontDestroyOnLoad (this.transform.root.gameObject);
		for (int count = 0; count < countOfSaves; count++) {
			saves.Add (new Save ());
		}
		instance = this;
		Deserialize ();
	}

	void Start() {
		LoadGraphicsSettings ();
		LoadInputSettings ();
	}

	void Update() {
		if (Input.GetKey (KeyCode.LeftBracket)) {
			Save (0, true, false);
		}

		if (Input.GetKey (KeyCode.RightBracket)) {
			Load (0);
		}
	}

	public void AutoSave() {
		Save (0, false, true);
	}

	public void Save(int index, bool quickSave = false, bool autoSave = false) {
		save = saves [index];
		save.name = System.DateTime.Now.ToString ();
		if (quickSave) {
			save.name += " Quick Save";
		} else if (autoSave) {
			save.name += " Auto Save";
		}
		save.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
		SaveQuests ();
		save.playerData = Player.instance.Save ();
		SaveDroppedItems ();
		//SaveScavs ();
		SaveDroppedWeapons ();
		Serialize ();
		save.scavsCountLimit = Scav.countLimit;
		saves [index] = save;
	}

	void OnGUI() {
		if (isLoading) {
			GUI.Box (new Rect (0, 0, Screen.width, Screen.height), Texture2D.blackTexture);
		}
	}

	private IEnumerator LoadAsync(int index) {
		if (saves[index] != null && saves[index].sceneName != "") {

			save = saves [index];
			if (save.sceneName != UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name) {
				yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (save.sceneName);
			}

			LoadQuests ();
			LoadDroppedItems ();
			//LoadScavs ();
			LoadDroppedWeapons ();

			//if (Player.instance != null) {
			//	Destroy (Player.instance.gameObject);
			//}
			//Instantiate (playerSpawnObject);
			Player.instance.Load (save.playerData);
		}
	}

	public void Load(int index) {
		//StartCoroutine (LoadAsync(index));
		if (saves[index] != null && saves[index].sceneName != "") {

			save = saves [index];
			if (save.sceneName != UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name) {
				//yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (save.sceneName);
				UnityEngine.SceneManagement.SceneManager.LoadScene(save.sceneName);
			}

			SkipDay skipDay = FindObjectOfType<SkipDay> ();
			if (skipDay != null) {
				skipDay.timer = 0.0f;
			}

			Scav[] scavs = FindObjectsOfType<Scav> ();
			Scav.count = 0;
			foreach (Scav scav in scavs) {
				Destroy (scav.gameObject);
			}

			LoadQuests ();
			LoadDroppedItems ();
			//LoadScavs ();
			LoadDroppedWeapons ();
			Scav.countLimit = save.scavsCountLimit;

			if (Player.instance == null) {
				Instantiate (playerSpawnObject);
			}
			Player.instance.killed = false;
			Player.instance.Load (save.playerData);

			GameObject obj = FindObjectOfType<DeathPlayerObject> ().gameObject;
			if (obj != null) {
				Destroy (obj);
			}
			Player.instance.GetComponentInChildren<Camera> ().enabled = true;
		}
	}

	public void SaveDroppedWeapons(){
		save.droppedWeaponsData.Clear ();
		Weapon[] weapons = FindObjectsOfType<Weapon> ();
		foreach (Weapon weapon in weapons) {
			if (weapon.transform.parent == null) {
				save.droppedWeaponsData.Add (weapon.Save ());
			}
		}
	}

	public void LoadDroppedWeapons(){
		Weapon[] weapons = FindObjectsOfType<Weapon> ();
		foreach (Weapon weapon in weapons) {
			if (weapon.transform.parent == null) {
				Destroy (weapon.gameObject);
			}
		}

		foreach (WeaponSaveData weaponSaveData in save.droppedWeaponsData) {
			Weapon weapon = LoadWeapon (weaponSaveData);
			weapon.GetComponent<Item> ().pickable = true;
			weapon.dropped = true;
			weapon.transform.position = weaponSaveData.position;
			weapon.transform.rotation = weaponSaveData.rotation;
		}
	}

	public void SaveScavs() {
		save.scavsCount = Scav.count;
		save.scavsData.Clear ();
		Scav[] scavs = FindObjectsOfType<Scav> ();
		foreach (Scav scav in scavs) {
			if (scav.character.killed) {
				continue;
			}
			save.scavsData.Add (scav.Save ());
		}
	}

	public void LoadScavs() {
		Scav.count = save.scavsCount;
		Scav[] scavs = FindObjectsOfType<Scav> ();
		foreach (Scav scav in scavs) {
			Destroy (scav.gameObject);
		}

		foreach (ScavSave scavSave in save.scavsData) {
			GameObject newScav = Instantiate (emptyScav);
			newScav.GetComponent<Scav> ().Load (scavSave);
		}
	}

	public void SaveDroppedItems() {
		save.droppedItemsData.Clear ();
		Item[] items = FindObjectsOfType<Item> ();
		foreach (Item item in items) {
			if (item.transform.parent == null) {
				save.droppedItemsData.Add (item.Save ());
			}
		}
	}

	public void LoadDroppedItems() {
		Item[] items = FindObjectsOfType<Item> ();
		foreach (Item item in items) {
			if (item.gameObject.transform.parent == null) {
				Destroy (item.gameObject);
			}
		}
		foreach (ItemSaveData itemSaveData in save.droppedItemsData) {
			GameObject itemObject;
			if (ItemManager.instance.items.TryGetValue (itemSaveData.keyName, out itemObject)) {
				GameObject newItemObject = Instantiate (itemObject);
				Item item = newItemObject.GetComponent<Item>();
				item.Load (itemSaveData, null);
			}
		}
	}

	public void SaveQuests() {
		save.questSystemSaveData = QuestSystem.instance.Save ();

		QuestGiver[] questGivers = FindObjectsOfType<QuestGiver> ();
		save.questGiversSaveData.Clear ();
		foreach (QuestGiver questGiver in questGivers) {
			save.questGiversSaveData.Add (questGiver.Save ());
		}

		QuestChest[] questChests = FindObjectsOfType<QuestChest> ();
		//save.questChestsSaveData.Clear ();
		foreach (QuestChest questChest in questChests) {
			save.questChestsSaveData.Add (questChest.Save ());
		}
	}

	public void LoadQuests() {
		//QuestGiver[] questGivers = FindObjectsOfType<QuestGiver> ();
		//int index = 0;
		//foreach (QuestGiver questGiver in questGivers) {
		//	questGiver.Load (save.questGiversSaveData [index]);
		//	index++;
		//}

		QuestChest[] questChests = FindObjectsOfType<QuestChest> ();
		//index = 0;
		foreach (QuestChest questChest in questChests) {
			for(int index = 0; index < save.questChestsSaveData.Count; index++){
				questChest.Load(save.questChestsSaveData[index]);
			}
			//index++;
		}

		QuestSystem.instance.Load (save.questSystemSaveData);
	}

	public void SaveDestroyableObjects() {
		save.destroyableObjectsSaveData.Clear ();
		DestroyableObject[] objects = FindObjectsOfType<DestroyableObject> ();
		foreach(DestroyableObject destroyableObject in objects){
			save.destroyableObjectsSaveData.Add(destroyableObject.Save());
		}
	}

	public void LoadDestroyableObjects() {
		save.destroyableObjectsSaveData.Clear ();
		DestroyableObject[] objects = FindObjectsOfType<DestroyableObject> ();
		int index = 0;
		foreach(DestroyableObject destroyableObject in objects){
			destroyableObject.Load (save.destroyableObjectsSaveData[index]);
			index++;
		}
	}

	public Weapon LoadWeapon(WeaponSaveData data, Transform beginTransform = null){
		WeaponGeneratorManager weaponGenerator = WeaponGeneratorManager.instance;
		Queue<WeaponDeserializationQueueData> queue = new Queue<WeaponDeserializationQueueData> ();
		WeaponDeserializationQueueData queueData = new WeaponDeserializationQueueData();
		//GameObject beginOnNullGameObject = null;
		//if (beginTransform == null) {
		//	beginOnNullGameObject = Instantiate (new GameObject ());
		//} else {
		//	beginOnNullGameObject = Instantiate (new GameObject (), beginTransform);
		//}
		//beginTransform = beginOnNullGameObject.transform;
		queueData.partData = data.rootPart;
		queueData.transform = beginTransform;
		queue.Enqueue (queueData);

		Transform rootTransform = null;
		while (queue.Count > 0) {
			WeaponDeserializationQueueData queuePartData = queue.Dequeue();
			WeaponPartSaveData partData = queuePartData.partData;

			GameObject partGameObject;
			try {
				//Debug.Log(partData.name + " " + partData.id);
				partGameObject = Instantiate (weaponGenerator.parts [partData.name] [partData.id], queuePartData.transform);
			} catch(System.Exception ex) {
				continue;
			}

			if (rootTransform == null) {
				rootTransform = partGameObject.transform;
			}

			WeaponPart weaponPartCurrent = partGameObject.GetComponent<WeaponPart> ();
			WeaponPartConnection parentConnection = partGameObject.GetComponentInParent<WeaponPartConnection> ();
			if (parentConnection != null) {
				parentConnection.filled = true;
				parentConnection.filledPart = weaponPartCurrent;
			}

			if (partData.connections != null) {
				WeaponPartConnection[] connections = partGameObject.GetComponentsInChildren<WeaponPartConnection> ();
				int index = 0;
				foreach (WeaponPartConnectionSaveData connection in partData.connections) {
					if (connection.filled) {
						WeaponDeserializationQueueData newQueueData = new WeaponDeserializationQueueData ();
						newQueueData.partData = connection.filledPart;
						newQueueData.transform = connections [index].transform;
						queue.Enqueue (newQueueData);
					}
					index++;
				}
			}
		}

		if (rootTransform == null) {
			rootTransform = beginTransform;
		}
		Weapon weapon = null; 
		weapon = rootTransform.GetComponent<Weapon> ();
		if (weapon == null) {
			weapon = rootTransform.GetComponentInChildren<Weapon> ();
		}
		if (weapon == null) {
			weapon = beginTransform.GetComponentInChildren<Weapon> ();
		}
		weapon.data = data.originalData.GetScriptableObject();
		weapon.originalData = data.originalData.GetScriptableObject ();
		weapon.botCase = data.botCase;
		weapon.dropped = data.dropped;
		weapon.currentAmmo = data.currentAmmo;
		weapon.rigidBody = weapon.GetComponent<Rigidbody> ();
		rootTransform.localScale = data.scale;
		weapon.Initialize ();
		weapon.item.Render ();
		return weapon;
	}

	public void SaveGraphicsSettings(GraphicsSettingsData data) {
		System.IO.File.WriteAllText("GraphicsSettings.save", JsonUtility.ToJson (data));
	}

	public void LoadGraphicsSettings() {
		string json = System.IO.File.ReadAllText ("GraphicsSettings.save");
		if (json.Length > 2) {
			GraphicsSettingsData data = JsonUtility.FromJson<GraphicsSettingsData> (json);
			GraphicsSettings.instance.Data = data;
		}
	}

	public void SaveInputSettings(InputManagerDictionary settings) {
		System.IO.File.WriteAllText("InputSettings.save", JsonUtility.ToJson (settings.GetSerializableVariant()));
	}

	public void LoadInputSettings() { 
		string json = System.IO.File.ReadAllText ("InputSettings.save");
		if (json.Length > 2) {
			SerializableKeyValues<string, int> data = JsonUtility.FromJson<SerializableKeyValues<string, int>> (json);
			InputManager.instance.keys.SetFromSerializableVariant(data);
		}
	}
}
