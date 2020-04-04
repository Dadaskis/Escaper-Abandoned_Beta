using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class PlayerSaveData {
	public Vector3 position;
	public Quaternion cameraRotation;
	public List<WeaponSaveData> weaponsInInventory = new List<WeaponSaveData>();
	public List<ItemSaveData> itemsInInventory = new List<ItemSaveData> ();
	public WeaponSaveData currentWeapon;
	public CharacterSaveData character;
}

public class Player : MonoBehaviour {

	public WeaponData weaponData;
	private GameObject weaponObject;
	public Weapon weapon;
	public HandsIKPass handsAnimation;
	public InventorySystem inventory;
	public GameObject weaponHolder;
	public GameObject inventoryObject;
	public Camera camera;
	public HandsIKPass hands;
	public Character character;
	public GameObject deathPlayer;
	public Image damagePanel;
	public Canvas uiCanvas;
	public Text usageText;

	public GameObject WeaponObject {
		set {
			weaponObject = value;
			weapon = weaponObject.GetComponent<Weapon> ();
			handsAnimation.manipulator = weaponObject;
		}
		get {
			return weaponObject;
		}
	}

	public static Player instance;

	public PlayerSaveData Save() {
		PlayerSaveData data = new PlayerSaveData ();
		data.cameraRotation = camera.transform.rotation;
		data.position = transform.position;
		data.character = character.Save ();
		//Weapon[] weapons = inventoryObject.GetComponentsInChildren<Weapon> ();
		for (int index = 0; index < inventoryObject.transform.childCount; index++) {
			Transform itemObject = inventoryObject.transform.GetChild (index);
			Weapon weapon = itemObject.GetComponent<Weapon> ();
			if (weapon != null) {
				data.weaponsInInventory.Add (weapon.Save ());
			} else {
				Item item = itemObject.GetComponent<Item> ();
				if (item != null) {
					if (item.keyName != null) {
						data.itemsInInventory.Add (item.Save ());
					}
				}
			}
		}
		if (weapon != null) {
			data.currentWeapon = weapon.Save ();
		}

		return data;
	}

	public void Load(PlayerSaveData data) {
		//camera.transform.rotation = data.cameraRotation;
		transform.position = data.position;
		character.Load (data.character);

		foreach(Inventory inventory in inventory.inventories) {
			foreach (Item item in inventory.items.ToArray ()) {
				item.RemoveFromInventory ();
			}
		}

		foreach (Container container in inventory.containers) {
			if (container.item != null) {
				Destroy (container.item.gameObject);
				//container.item.RemoveFromInventory ();
				container.item = null;
			}
		}

		foreach (WeaponSaveData weaponSaveData in data.weaponsInInventory) {
			Weapon weapon = SaveSystem.instance.LoadWeapon (weaponSaveData, inventoryObject.transform);
			weapon.gameObject.SetActive (false);
			Item item = weapon.GetComponent<Item> ();
			item.Load (weaponSaveData.item, inventory);
		}

		foreach (ItemSaveData itemSaveData in data.itemsInInventory) {
			GameObject itemObject = Instantiate (ItemManager.instance.items [itemSaveData.keyName], inventoryObject.transform);
			Item item = itemObject.GetComponent<Item> ();
			item.Load (itemSaveData, inventory);
			item.gameObject.SetActive (false);
		}

		if (data.currentWeapon.data != null) {
			if (weapon != null) {
				Destroy (weapon.gameObject);
				//for (int index = 0; index < weaponHolder.transform.childCount; index++) {
				//	GameObject obj = weaponHolder.transform.GetChild (index).gameObject;
				//	Destroy (obj);
				//}
				//Transform weaponHolderParent = weaponHolder.transform.parent;
				//Destroy (weaponHolder);
				//weaponHolder = Instantiate (new GameObject ("WeaponHolder"), weaponHolderParent);

			}
			weapon = SaveSystem.instance.LoadWeapon (data.currentWeapon, weaponHolder.transform);
			if (weapon != null) {
				weaponObject = weapon.gameObject;
				weapon.dropped = false;
				weapon.botCase = false;
				weapon.rigidBody.useGravity = false;
				weapon.rigidBody.isKinematic = true;
				handsAnimation.manipulator = weapon.gameObject;
				inventory.containers [0].item = weapon.item;
				weapon.gameObject.layer = LayerMask.NameToLayer ("Gun layer");
				MeshRenderer[] renderers = weapon.GetComponentsInChildren<MeshRenderer> ();
				if (renderers != null) {
					//for (int index = 0; index < weapon.transform.childCount; index++) {
					//weapon.transform.GetChild (index).gameObject.layer = LayerMask.NameToLayer ("Gun layer");
					//}
					foreach (MeshRenderer renderer in renderers) {
						renderer.gameObject.layer = LayerMask.NameToLayer ("Gun layer");
					}
				}
			}
		}
	}

	public bool killed = false;
	void OnDeath() {
		if(!killed) {
			GameObject player = Instantiate (deathPlayer, transform);
			player.transform.SetParent (null);
			GetComponentInChildren<Camera> ().enabled = false;
			Destroy (this.gameObject);
			//gameObject.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
			killed = true;
		}
	}

	void Awake() {
		DontDestroyOnLoad (this);
		instance = this;
		character.onDeath.AddListener (OnDeath);
	}

	void Update() {
		Item weaponItem = inventory.containers [0].item;
		if (weaponItem != null && weapon == null) {
			Weapon weapon = weaponItem.GetComponent<Weapon> ();
			if (weapon != null) {
				this.weapon = weapon;
				weapon.transform.localPosition = Vector3.zero;
				weaponItem.gameObject.transform.parent = weaponHolder.transform;
				weaponItem.gameObject.SetActive (true);
				weaponItem.pickable = false;
				weapon.dropped = false;
				weapon.botCase = false;
				Rigidbody body = weapon.rigidBody;
				body.detectCollisions = false;
				body.isKinematic = true;
				body.useGravity = false;
				weaponObject = weaponItem.gameObject;
				hands.manipulator = weaponObject;
				weapon.Initialize ();
				weapon.gameObject.layer = LayerMask.NameToLayer ("Gun layer");
				MeshRenderer[] renderers = weapon.GetComponentsInChildren<MeshRenderer> ();
				if (renderers != null) {
					//for (int index = 0; index < weapon.transform.childCount; index++) {
						//weapon.transform.GetChild (index).gameObject.layer = LayerMask.NameToLayer ("Gun layer");
					//}
					foreach (MeshRenderer renderer in renderers) {
						renderer.gameObject.layer = LayerMask.NameToLayer ("Gun layer");
					}
				}
			}
		} else if (weaponItem == null && weapon != null) {
			//Weapon weapon = weaponObject.GetComponent<Weapon> ();
			weapon.rigidBody.detectCollisions = true;
			weapon.gameObject.layer = LayerMask.NameToLayer ("Default");
			MeshRenderer[] renderers = weapon.GetComponentsInChildren<MeshRenderer> ();
			if (renderers != null) {
				//for (int index = 0; index < weapon.transform.childCount; index++) {
				//weapon.transform.GetChild (index).gameObject.layer = LayerMask.NameToLayer ("Gun layer");
				//}
				foreach (MeshRenderer renderer in renderers) {
					renderer.gameObject.layer = LayerMask.NameToLayer ("Default");
				}
			}
			hands.manipulator = null;
			if (weapon.dropped) {
				weaponObject = null;
				weapon = null;
				return;
			}
			weapon.gameObject.SetActive (false);
			weapon.gameObject.transform.parent = inventory.transform;
			weaponObject = null;
			hands.manipulator = null;
			weapon.dropped = true;
			this.weapon = null;
		}

		usageText.text = "";
		RaycastHit hit;
		if (Physics.Raycast (camera.transform.position + camera.transform.forward, camera.transform.forward, out hit, 2.0f)) {
			QuestChest questChest = hit.transform.root.GetComponent<QuestChest> ();
			if (questChest != null) {
				usageText.text = "[" + System.Enum.GetName (typeof(KeyCode), InputManager.instance.keys ["Use"]) + "] put";
			}

			Item item = hit.transform.root.GetComponentInChildren<Item> ();
			if (item != null) {
				usageText.text = "[" + System.Enum.GetName (typeof(KeyCode), InputManager.instance.keys ["Use"]) + "] take";
			}

			LootBox box = hit.transform.GetComponent<LootBox> ();
			if (box != null) {
				usageText.text = "[" + System.Enum.GetName (typeof(KeyCode), InputManager.instance.keys ["Use"]) + "] loot";
			}

			ExplodeOnUse explode = hit.transform.GetComponent<ExplodeOnUse> ();
			if (explode != null) {
				usageText.text = "[" + System.Enum.GetName (typeof(KeyCode), InputManager.instance.keys ["Use"]) + "] explode";
			}

			OpenOnUse open = hit.transform.GetComponent<OpenOnUse> ();
			if (open != null) {
				usageText.text = "[" + System.Enum.GetName (typeof(KeyCode), InputManager.instance.keys ["Use"]) + "] open";
			}

			if (InputManager.GetButtonDown("Use")) {
				if (questChest != null) {
					questChest.Use ();
				}

				if (item != null) {
					if (item.pickable) {
						foreach (Inventory inventory2 in inventory.inventories) {
							if (inventory2.AddItem (item)) {
								item.pickable = false;
								item.gameObject.SetActive (false);
								item.transform.parent = inventory.transform;
								item.Render ();
								return;
							} else {
								item.Rotate ();
								if (inventory2.AddItem (item)) {
									item.pickable = false;
									item.gameObject.SetActive (false);
									item.transform.parent = inventory.transform;
									item.Render ();
									return;
								} else {
									item.Rotate ();
								}
							}
						}
					}
				}

				if (box != null) {
					box.Use ();
				}

				if (explode != null) {
					explode.Use ();
				}

				if (open != null) {
					open.Use ();
				}
			}
		}
	}

}
