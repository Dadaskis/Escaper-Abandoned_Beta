using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

[System.Serializable]
public class SlotSaveData {
	public ItemSaveData item;
	public bool haveItem;
}

[System.Serializable]
public class Slot {
	public Item item;
	public bool haveItem;
	public Inventory inventory;
}

[System.Serializable]
public class InventorySaveData {
	public SlotSaveData[,] slots;
	public List<ItemSaveData> items;
	public int slotsWidth;
	public int slotsHeight;
}

[System.Serializable]
public class Inventory {
	public int ID = -1;
	public Texture slotTexture;
	public float size;
	public Slot[,] slots;
	public int slotsWidth = 10;
	public int slotsHeight = 5;
	public List<Item> items = new List<Item> ();
	public Vector2 position;

	public InventorySystem system;

	private Rect rect;

	bool CheckSlots(int xBegin, int yBegin, int xEnd, int yEnd){
		if (
			(xBegin < 0 || yBegin < 0) 
			||
			(xEnd < 0 || yEnd < 0)
			|| 
			(xEnd > slots.GetLength(0) || yEnd > slots.GetLength(1))
			||
			(xBegin > slots.GetLength(0) || yBegin > slots.GetLength(1))
		) {
			return false;
		}
		for (int X = xBegin; X < xEnd; X++) {
			for (int Y = yBegin; Y < yEnd; Y++) {
				if (slots [X, Y].haveItem) {
					return false;
				}
			}
		}
		return true;
	}

	void MarkSlots(Item item){
		for(int X = (int)item.position.x; X < (int)item.position.x + (int)item.size.x; X++){
			for (int Y = (int)item.position.y; Y < (int)item.position.y + (int)item.size.y; Y++) {
				slots [X, Y].haveItem = true;
				slots [X, Y].item = item;
			}
		}
	}

	void UnmarkSlots(Item item){
		for(int X = (int)item.position.x; X < (int)item.position.x + (int)item.size.x; X++){
			for (int Y = (int)item.position.y; Y < (int)item.position.y + (int)item.size.y; Y++) {
				slots [X, Y].haveItem = false;
				slots [X, Y].item = null;
			}
		}
	}

	void SetupItem(Item item){
		if (
			CheckSlots (
				(int)item.position.x, 
				(int)item.position.y,
				(int)item.position.x + (int)item.size.x, 
				(int)item.position.y + (int)item.size.y
			)
		) {
			MarkSlots (item);
		}
	}

	public bool AddItem(Item item){
		bool havePlace = false;
		int freeX = 0;
		int freeY = 0;
		if (
			CheckSlots (
				(int)item.position.x, 
				(int)item.position.y,
				(int)item.position.x + (int)item.size.x, 
				(int)item.position.y + (int)item.size.y
			)
		) 
		{
			havePlace = true;
			freeX = (int)item.position.x;
			freeY = (int)item.position.y;
		} else {
			for (int X = 0; X < slotsWidth - ((int)item.size.x - 1); X++) {
				for (int Y = 0; Y < slotsHeight - ((int)item.size.y - 1); Y++) {
					if (CheckSlots (X, Y, X + (int)item.size.x, Y + (int)item.size.y)) {
						havePlace = true;
						freeX = X;
						freeY = Y;
						break;
					}
				}
			}
		}
		if (havePlace) {
			items.Add (item);
			item.position.x = (float)freeX;
			item.position.y = (float)freeY;
			MarkSlots (item);
			item.inventory = this;
			reRenderItems = true;
			return true;
		}
		return false;
	}

	public void RemoveItem(Slot slot) {
		if (!slot.haveItem) {
			return;
		}
		Item item = slot.item;
		UnmarkSlots (item);
		items.Remove (item);
		item.inventory = null;
		reRenderItems = true;
	}

	public void RemoveItem(Item item){
		RemoveItem (slots [(int)item.position.x, (int)item.position.y]);
	}

	public void Start() {
		rect = new Rect ();
		slots = new Slot[slotsWidth, slotsHeight];
		for (int X = 0; X < slotsWidth; X++) {
			for (int Y = 0; Y < slotsHeight; Y++) {
				slots [X, Y] = new Slot ();
				slots [X, Y].inventory = this;
			}
		}
		if (items.Count > 0) {
			foreach (Item item in items) {
				SetupItem (item);
			}
		}
	}

	private Rect checkCursorRect = new Rect ();
	private Vector2 mousePosition2D = new Vector2();
	bool IsCursorOnSlot(int X, int Y) {
		int pixelsSize = Mathf.RoundToInt(Screen.height * size);
		Vector3 mousePosition = Input.mousePosition;
		float xPos = groupRect.x + pixelsSize * X;
		float yPos = groupRect.y + pixelsSize * Y;
		checkCursorRect.x = xPos;
		checkCursorRect.y = yPos;
		checkCursorRect.width = pixelsSize;
		checkCursorRect.height = pixelsSize;
		mousePosition2D.x = mousePosition.x;
		mousePosition2D.y = Screen.height - mousePosition.y;
		if(checkCursorRect.Contains(mousePosition2D)) {
			return true;
		}
		return false;
	}

	bool IsCursorOnInventory(){
		int pixelsSize = Mathf.RoundToInt(Screen.height * size);
		Vector3 mousePosition = Input.mousePosition;
		mousePosition2D.x = mousePosition.x;
		mousePosition2D.y = Screen.height - mousePosition.y;
		if (groupRect.Contains (mousePosition2D)) {
			return true;
		}
		return false;
	}

	void DrawSlots(){
		int pixelsSize = Mathf.RoundToInt(Screen.height * size);
		rect.width = pixelsSize;
		rect.height = pixelsSize;
		for (int X = 0; X < slotsWidth; X++) {
			for (int Y = 0; Y < slotsHeight; Y++) {
				rect.x = pixelsSize * X;
				rect.y = pixelsSize * Y;
				GUI.DrawTexture (rect, slotTexture);
			}
		}
	}

	void DrawItems() {
		foreach (Item item in items) {
			item.Draw (size);
		}
	}

	private Rect groupRect = new Rect(0, 0, 0, 0);
	private bool isMouseDown = false;
	private bool isMouseUp = false;
	private bool reRenderItems = false;

	public void Update() {
		if (Input.GetMouseButtonDown (0)) {
			isMouseDown = true;
			isMouseUp = false;
		}
		if (Input.GetMouseButtonUp (0)) {
			isMouseDown = false;
			isMouseUp = true;
		}
	}

	public void Draw () {
		int pixelsSize = Mathf.RoundToInt(Screen.height * size);
		groupRect.x = Screen.width * position.x;
		groupRect.y = Screen.height * position.y;
		groupRect.width = pixelsSize * slotsWidth;
		groupRect.height = pixelsSize * slotsHeight;

		GUI.BeginGroup (groupRect);
		DrawSlots ();
		DrawItems ();
		GUI.EndGroup ();

		if (IsCursorOnInventory ()) {
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.x -= groupRect.x;
			mousePosition.y = (Screen.height - mousePosition.y) - groupRect.y;
			int X = (int)mousePosition.x / pixelsSize;
			int Y = (int)mousePosition.y / pixelsSize;

			if(isMouseDown && !system.draggingItem){
				if (slots [X, Y].haveItem) {
					system.dragItem = slots [X, Y].item;
					RemoveItem (slots [X, Y]);
					system.draggingItem = true;
				}
			}

			if (isMouseUp) {
				if (system.draggingItem) {
					if (slots[X, Y].haveItem) {
						Item slotItem = slots [X, Y].item;
						if (slotItem.tag == system.dragItem.tag 
							&& slotItem.maxCount > 1
							&& slotItem.currentCount != slotItem.maxCount) {
							int neededToMax = slotItem.maxCount - slotItem.currentCount;
							if (system.dragItem.currentCount > neededToMax) {
								slotItem.currentCount = slotItem.maxCount;
								system.dragItem.currentCount -= neededToMax;
							} else {
								slotItem.currentCount = slotItem.maxCount;
								system.DestroyDragItem ();
							}
							return;
						}
					}
					Item dragItem = system.dragItem;
					dragItem.position.x = X;
					dragItem.position.y = Y;
					if (AddItem (dragItem)) {
						system.dragItem = null;
						system.draggingItem = false;
					}
				}
			}
		}
	}
}

[System.Serializable]
public class ContainerSaveData {
	public ItemSaveData item;
}

[System.Serializable]
public class Container {
	public int ID = -1;
	public Vector2 position;
	public Vector2 size;
	public Texture texture;
	public Item item;
	public InventorySystem system;
	public bool isMouseDown;
	public bool isMouseUp;
	public bool weaponOnly = true;

	private Vector2 mousePosition2D;
	private Rect rect;

	bool IsCursorOnContainer(){
		Vector3 mousePosition = Input.mousePosition;
		mousePosition2D.x = mousePosition.x;
		mousePosition2D.y = Screen.height - mousePosition.y;
		if (rect.Contains (mousePosition2D)) {
			return true;
		}
		return false;
	}

	public void Update() {
		if (Input.GetMouseButtonDown (0)) {
			isMouseDown = true;
			isMouseUp = false;
		}
		if (Input.GetMouseButtonUp (0)) {
			isMouseDown = false;
			isMouseUp = true;
		}
	}

	public void Draw () {
		rect = new Rect (Screen.width * position.x, Screen.height * position.y, Screen.width * size.x, Screen.height * size.y);

		GUI.DrawTexture (rect, texture);

		if (weaponOnly && item == null) {
			GUI.Label(new Rect(rect.x + Screen.width * 0.02f, rect.y + Screen.height * 0.02f, rect.width, rect.height), "Put weapon here", InventorySystem.instance.smallFont);
		}

		if (item != null) {
			item.Draw (rect);
		}

		if (IsCursorOnContainer ()) {
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.x -= rect.x;
			mousePosition.y = (Screen.height - mousePosition.y) - rect.y;

			if(isMouseDown && !system.draggingItem){
				if (item != null) {
					system.dragItem = item;
					item.container = null;
					item = null;
					system.draggingItem = true;
				}
			}

			if (isMouseUp) {
				if (system.draggingItem && item == null) {
					if (weaponOnly) {
						Weapon weapon = system.dragItem.GetComponent<Weapon> ();
						if (weapon == null) {
							return;
						}
					}
					item = system.dragItem;
					item.container = this;
					system.dragItem = null;
					system.draggingItem = false;
				}
			}
		}
	}
}

[System.Serializable]
public class InventorySystemSaveData {
	public List<InventorySaveData> inventories;
	public List<ContainerSaveData> containers; 
} 

public class InventorySystem : MonoBehaviour {
	public List<Inventory> inventories;
	public List<Container> containers;
	public float size;
	public bool isEnabled;
	public bool isFirstEnabled;
	public Texture slotTexture;
	public FirstPersonController controller;
	public GUIStyle style;

	public GUIStyle smallFont = new GUIStyle ();

	public static InventorySystem instance;

	void Awake() {
		instance = this;
		int counter = 0;
		foreach (Inventory inventory in inventories) {
			inventory.ID = counter++;
			inventory.system = this;
			inventory.size = size;
			inventory.Start ();
		}

		counter = 0;
		foreach (Container container in containers) {
			container.ID = counter++;
			container.system = this;
		}

		smallFont.normal.textColor = new Color (1.0f, 1.0f, 1.0f);
		smallFont.active.textColor = new Color (1.0f, 1.0f, 1.0f);
		smallFont.focused.textColor = new Color (1.0f, 1.0f, 1.0f);
		smallFont.onNormal.textColor = new Color (1.0f, 1.0f, 1.0f);
		smallFont.onActive.textColor = new Color (1.0f, 1.0f, 1.0f);
		smallFont.onFocused.textColor = new Color (1.0f, 1.0f, 1.0f);
		smallFont.wordWrap = true;
	}

	public List<Item> FindItemsWithTagInInventory(string tag){
		List<Item> items = new List<Item> ();
		foreach (Inventory inventory in inventories) {
			foreach (Item item in inventory.items) {
				if (item.tag == tag) {
					items.Add (item);
				}
			}
		}
		return items;
	}

	public Item FindItemWithTagInInventory(string tag){
		foreach (Inventory inventory in inventories) {
			foreach (Item item in inventory.items) {
				if (item.tag == tag) {
					return item;
				}
			}
		}
		return null;
	}

	void Update() {
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name == "Main menu") {
			isEnabled = false;
			return;
		}
		if (InGameMenu.instance.isOpened) {
			return;
		}
		style.fontSize = (int)(Screen.height * 0.04f);
		smallFont.fontSize = (int)(Screen.height * 0.025f);
		if (InputManager.GetButtonDown("InventoryOpen")) {
			isEnabled = !isEnabled;
			isFirstEnabled = isEnabled;
			controller.enableMouseLook = !isEnabled;
			controller.mouseLook.SetCursorLock (!isEnabled);
			foreach (Inventory inventory in inventories) {
				foreach (Item item in inventory.items) {
					item.Render ();
				}
			}

			foreach (Container container in containers) {
				if (container.item != null) {
					container.item.Render ();
				}
			}
		}
		if (isEnabled) {
			foreach (Inventory inventory in inventories) {
				inventory.Update ();
			}

			foreach (Container container in containers) {
				container.Update ();
			}

			if (InputManager.GetButtonDown ("RotateItem")) {
				rotateDragItem = true;
			}
		}
	}

	void OnGUI() {
		if (isEnabled) {			
			foreach (Inventory inventory in inventories) {
				inventory.Draw ();
			}

			foreach (Container container in containers) {
				container.Draw ();
			}

			DrawInfo ();
			DrawDragItem ();
			DrawQuests ();

			isFirstEnabled = false;
		}
	}

	void DrawQuests() {
		float groupWidth = Screen.width * 0.30f;
		float groupHeight = Screen.height * 0.65f;
		GUI.BeginGroup (new Rect(
			Screen.width * 0.01f,
			Screen.height * 0.15f,
			groupWidth,
			groupHeight
		));

		GUI.DrawTexture (new Rect (0, 0, groupWidth, groupHeight), slotTexture);
		GUI.Label (new Rect (Screen.width * 0.01f, Screen.height * 0.01f, groupWidth, Screen.height * 0.05f), "Quests", smallFont); 

		string text = "";
		int counter = 1;
		bool haveQuests = false;
		foreach (Quest quest in QuestSystem.instance.quests) {
			text += "\n\n" + counter + ": " + quest.displayName;
			counter++;
			haveQuests = true;
		}
		if (haveQuests == false) {
			text = "\n\nNow you can go to base!";
		}

		GUI.Label (new Rect (Screen.width * 0.01f, Screen.height * 0.06f, groupWidth, groupHeight), text, smallFont); 

		GUI.EndGroup ();
	}

	void DrawInfo() {
		float groupWidth = Screen.width * 0.35f;
		float groupHeight = Screen.height * 0.55f;

		GUI.BeginGroup (new Rect(
			Screen.width * 0.35f,
			Screen.height * 0.15f,
			groupWidth,
			groupHeight
		));

		GUI.DrawTexture (new Rect (0, 0, groupWidth, groupHeight), slotTexture);

		if (previousDragItem != null && dragItem == null) {
			
			//previousDragItem.Draw(
			//	new Vector2(
			//		groupWidth * (0.3f - (0.03f * previousDragItem.size.x)),
			//		groupHeight * (0.2f - (0.03f * previousDragItem.size.y))
			//	),
			//	(previousDragItem.size.x / previousDragItem.size.y) / 30.0f
			//);

			GUI.Label (new Rect (groupWidth * 0.1f, groupHeight * 0.4f, groupWidth * 0.8f, groupHeight * 0.1f), previousDragItem.name);

			if (previousDragItem.maxCount > 1) {
				GUI.Label (new Rect (groupWidth * 0.1f, groupHeight * 0.5f, groupWidth * 0.8f, groupHeight * 0.1f), 
					previousDragItem.currentCount + " / " + previousDragItem.maxCount);
			}

			if (GUI.Button (new Rect (groupWidth * 0.1f, groupHeight * 0.6f, groupWidth * 0.8f, groupHeight * 0.08f), "Drop", style)) {
				Weapon weapon = previousDragItem.GetComponent<Weapon> ();
				if (weapon != null) {
					weapon.dropped = true;
					Rigidbody body = weapon.GetComponent<Rigidbody> ();
					weapon.rigidBody.useGravity = true;
					weapon.rigidBody.isKinematic = false;
					weapon.rigidBody.detectCollisions = true;

					if(previousDragItem.container != null && previousDragItem.container.item == previousDragItem) {
						Player.instance.hands.manipulator = null;
					}
				}

				previousDragItem.transform.parent = null;
				previousDragItem.transform.position = transform.position + transform.forward;
				previousDragItem.gameObject.SetActive (true);
				previousDragItem.pickable = true;

				if (previousDragItem.inventory != null) {
					previousDragItem.RemoveFromInventory (false);
				} else if (previousDragItem.container != null) {
					previousDragItem.container.item = null;
					previousDragItem.container = null;
				} 

				previousDragItem = null;
			}

			if (previousDragItem != null) {
				if (previousDragItem.usable != null) {
					if (GUI.Button (new Rect (groupWidth * 0.1f, groupHeight * 0.7f, groupWidth * 0.8f, groupHeight * 0.08f), "Use", style)) {
						previousDragItem.usable.Use (previousDragItem);
					}
				}
			}

		}

		GUI.EndGroup ();
	}

	public Item dragItem = null;
	public bool draggingItem = false;
	public Item previousDragItem = null;
	private Vector2 mousePosition2D;
	private bool rotateDragItem = false;

	void DrawDragItem() {
		if (draggingItem) {
			previousDragItem = dragItem;
			Vector3 mousePosition = Input.mousePosition;
			mousePosition2D.x = mousePosition.x;
			mousePosition2D.y = Screen.height - mousePosition.y;
			if (rotateDragItem) {
				rotateDragItem = false;
				dragItem.Rotate ();
			}
			dragItem.Draw (mousePosition2D, size);
		}
	}

	public void DestroyDragItem(){
		draggingItem = false;
		Destroy (dragItem.gameObject);
		dragItem = null;
	}
}
