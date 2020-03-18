using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSaveData {
	public int inventoryID = -1;
	public int containerID = -1;
	public Vector2 size = Vector2.one;
	public Vector2 position = Vector2.zero;
	public Quaternion rotation = Quaternion.identity;
	public float offsetX = 0.0f;
	public float offsetZ = 0.0f;
	public bool pickable = true;
	public string name;
	public string tag;
	public bool rotated = false;
	public int maxCount = 1;
	public int currentCount = 1;
	public string keyName;
	public Vector3 worldPosition = Vector3.zero;
	public bool removable = true;
	public float sizeMultiplier = 1.0f;
}

public class Item : MonoBehaviour {
	public Inventory inventory;
	public Container container;
	public Vector2 size = Vector2.one;
	public Vector2 position = Vector2.zero;
	public Quaternion rotation = Quaternion.identity;
	public float offsetX = 0.0f;
	public float offsetZ = 0.0f;
	public bool pickable = true;
	public bool rotated = false;
	public string name;
	public string keyName;
	public string tag;
	public float zoomPower = 14.0f;
	public int maxCount = 1;
	public int currentCount = 1;
	public Usable usable;
	public bool removable = true;
	public float sizeMultiplier = 1.0f;
	public Texture border;

	private RenderTexture renderTexture;

	private Rect rect = new Rect();

	void Start(){
		Resize();
		rotation = Quaternion.Euler (new Vector3 (0.0f, 90.0f, 0.0f));
	}

	public ItemSaveData Save() {
		ItemSaveData data = new ItemSaveData ();

		data.size = size;
		data.position = position;
		data.rotation = rotation;
		data.offsetX = offsetX;
		data.offsetZ = offsetZ;
		data.pickable = pickable;
		data.name = name;
		data.tag = tag;
		data.rotated = rotated;
		data.currentCount = currentCount;
		data.maxCount = maxCount;
		data.keyName = keyName;
		data.worldPosition = transform.position;
		data.removable = removable;
		data.sizeMultiplier = sizeMultiplier;

		if (container != null) {
			data.containerID = container.ID;
		}
		if (inventory != null) {
			data.inventoryID = inventory.ID;
		}

		return data;
	}

	public void Load(ItemSaveData data, InventorySystem system) {
		size = data.size;
		position = data.position;
		rotation = data.rotation;
		offsetX = data.offsetX;
		offsetZ = data.offsetZ;
		pickable = data.pickable;
		name = data.name;
		tag = data.tag;	
		rotated = data.rotated;
		currentCount = data.currentCount;
		maxCount = data.maxCount;
		keyName = data.keyName;
		transform.position = data.worldPosition;
		removable = data.removable;
		sizeMultiplier = data.sizeMultiplier;

		if (data.inventoryID > -1) {
			inventory = system.inventories [data.inventoryID];
			inventory.AddItem (this);
		}

		if (data.containerID > -1) {
			container = system.containers [data.containerID];
			container.item = this;
		}

		Render ();
	}

	public Bounds CalculateLocalBounds() {
		Quaternion currentRotation = transform.rotation;
		transform.rotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
		BoxCollider[] colliders = GetComponentsInChildren<BoxCollider>();
		Bounds bounds = colliders [0].bounds;
		foreach (BoxCollider collider in colliders) {
			bounds.Encapsulate (collider.bounds);
		}
		Vector3 localCenter = bounds.center - transform.position;
		bounds.center = localCenter;
		transform.rotation = currentRotation;
		return bounds;
	}

	public void Rotate() {
		float temp = size.x;
		size.x = size.y;
		size.y = temp;
		if (rotated) {
			rotated = false;
			rotation = Quaternion.Euler (new Vector3 (0.0f, 90.0f, 0.0f));
			offsetZ /= 2.0f;
		} else {
			rotated = true;
			rotation = Quaternion.Euler (new Vector3 (0.0f, 90.0f, 90.0f));
			offsetZ *= 2.0f;
		}
		Render ();
	}

	public void Render() {
		if (renderTexture == null) {
			renderTexture = new RenderTexture (128, 128, 24);
		}
		ItemRenderer.instance.camera.aspect = size.x / size.y;
		ItemRenderer.instance.camera.targetTexture = renderTexture;
		ItemRenderer.instance.RenderMesh (gameObject, rotation, offsetZ, offsetX);
		ItemRenderer.instance.camera.targetTexture = ItemRenderer.instance.renderTexture;
		ItemRenderer.instance.camera.aspect = 1.0f;
	}

	public void Render(float aspect) {
		if (renderTexture == null) {
			renderTexture = new RenderTexture (128, 128, 24);
		}
		ItemRenderer.instance.camera.aspect = aspect;
		ItemRenderer.instance.camera.targetTexture = renderTexture;
		ItemRenderer.instance.RenderMesh (gameObject, rotation, offsetZ, offsetX);
		ItemRenderer.instance.camera.targetTexture = ItemRenderer.instance.renderTexture;
		ItemRenderer.instance.camera.aspect = 1.0f;
	}

	public void Resize() {
		Resize (CalculateLocalBounds ());
	}

	public void Resize(Bounds bounds) {
		Vector3 extents = bounds.extents;
		size.x = Mathf.RoundToInt(extents.z * zoomPower * sizeMultiplier);
		size.y = Mathf.RoundToInt(extents.y * zoomPower * sizeMultiplier);
		if (size.x < 1.0f) {
			size.x = 1.0f;
		}
		if (size.y < 1.0f) {
			size.y = 1.0f;
		}
		offsetX = -bounds.center.z;
		offsetZ = -extents.z * (zoomPower * 0.11f);
		/*
		Debug.Log ("Center: " + bounds.center);
		Debug.Log ("Extents: " + bounds.extents);
		Debug.Log ("Offset: " + "X: " + offsetX + ". Z: " + offsetZ);
		Debug.Log ("Rotation: " + rotation);*/
	}

	public void Draw(float slotSize) {
		int pixelsSize = Mathf.RoundToInt (slotSize * Screen.height);
		rect.x = pixelsSize * position.x;
		rect.y = pixelsSize * position.y;
		rect.width = Screen.height * (size.x * slotSize);
		rect.height = Screen.height * (size.y * slotSize);
		GUI.DrawTexture (rect, renderTexture);
		GUI.DrawTexture (rect, ItemRenderer.instance.borderTexture);
	}

	public void Draw(Vector2 position, float slotSize){
		rect.x = position.x;
		rect.y = position.y;
		rect.width = Screen.height * (size.x * slotSize);
		rect.height = Screen.height * (size.y * slotSize);
		GUI.DrawTexture (rect, renderTexture);
		GUI.DrawTexture (rect, ItemRenderer.instance.borderTexture);
	}

	public void Draw(Rect rect){
		GUI.DrawTexture (rect, renderTexture);
		GUI.DrawTexture (rect, ItemRenderer.instance.borderTexture);
	}

	public void RemoveFromInventory(bool destroy = true){
		if (inventory.ID != -1) {
			inventory.RemoveItem (this);
			if (destroy) {
				Destroy (this.gameObject);
			}
		}
	}
}
