using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponSaveData {
	public WeaponDataSerializable data = null;
	public WeaponDataSerializable originalData = null;
	public WeaponPartSaveData rootPart = null;
	public bool botCase = false;
	public bool dropped = false;
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
	public ItemSaveData item = null;
	public int currentAmmo = 0;
}

public class Weapon : MonoBehaviour {
	public Rigidbody rigidBody;
	public WeaponData data;
	public WeaponData originalData;
	public WeaponPart rootPart;
	public GameObject hitObject;
	public GameObject bulletObject;
	public LeftHand leftHand;
	public RightHand rightHand;
	public ShootPos shootPos;
	public WeaponMag mag; 
	public WeaponBolt bolt;
	public WeaponSightPosition sight;
	public bool inSight;
	public Bounds bounds;
	public bool botCase = false;
	public bool dropped = false;
	public View view;
	public Item item;
	public Character owner = null;
	public int currentAmmo = 0;
	public InventorySystem inventory;
	public bool reloading = false;
	public int shootBulletsCount = 1;

	private float nextTimeShoot = 0.0f;
	private Vector3 currentRecoil = new Vector3 (0.0f, 0.0f, 1.0f);
	private Vector3 zeroRecoil = new Vector3 (0.0f, 0.0f, 1.0f);
	private float reloadCounter = 0.0f;
	private Vector3 leftHandOrigin;
	private Vector3 rightHandOrigin;
	private Vector3 boltOrigin;
	private Vector3 magOrigin;
	private Vector3 weaponOrigin;
	private Quaternion leftHandOriginRotation;
	private Quaternion rightHandOriginRotation;
	private Quaternion magOriginRotation;
	private Quaternion reloadRotation;
	private int reloadStage;
	private int reloadAmmo;

	public void Initialize() {
		Weapon weapon = this;
		WeaponData data = weapon.data;
		Vector3 initPosition = weapon.transform.position;
		rootPart.weapon = weapon;
		rootPart.FillUp ();
		weapon.bounds = weapon.CalculateLocalBounds ();
		data.idlePosition.x = 0.1f;
		data.idlePosition.y = weapon.bounds.min.y / 2.0f;
		data.idlePosition.z = weapon.bounds.max.z / 1.7f;
		weapon.transform.localPosition = Vector3.zero;
		Vector3 sightOffset = Vector3.zero;
		Quaternion previousRotation = weapon.transform.rotation;
		weapon.transform.rotation = Quaternion.identity;
		sightOffset = weapon.transform.position - weapon.sight.transform.position;
		sightOffset.z = data.idlePosition.z * 0.8f;
		weapon.transform.rotation = previousRotation;
		data.sightPosition = sightOffset;
		Item item = weapon.GetComponent<Item> ();
		item.Resize ();
		transform.position = initPosition;
		owner = transform.root.GetComponent<Character> ();
		//currentAmmo = mag.ammoCount;
		inventory = transform.root.GetComponentInChildren<InventorySystem> ();
		leftHandOrigin = leftHand.transform.localPosition;
		rightHandOrigin = rightHand.transform.localPosition;
		magOrigin = mag.transform.localPosition;
		WeaponCollimatorSight redDot = GetComponentInChildren<WeaponCollimatorSight> ();
		if (redDot != null) {
			redDot.weapon = weapon;
			if (weapon.shootPos != null) {
				redDot.UpdatePosition ();
			}
		}
	}

	public WeaponSaveData Save() {
		WeaponSaveData data = new WeaponSaveData ();

		data.botCase = botCase;
		data.data = new WeaponDataSerializable();
		data.data.CopyFrom (this.originalData);
		data.originalData = new WeaponDataSerializable ();
		data.originalData.CopyFrom (this.originalData);
		data.dropped = dropped;
		data.rootPart = rootPart.Save ();
		data.position = transform.position;
		data.rotation = transform.rotation;
		data.scale = transform.localScale;
		data.item = GetComponent<Item> ().Save ();
		data.currentAmmo = currentAmmo;

		return data;
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

	void Recoil() {
		Quaternion rotationNative = transform.rotation;
		currentRecoil.y += Random.Range (-data.downRecoil, data.upRecoil);
		currentRecoil.x += Random.Range (-data.leftRecoil, data.rightRecoil);

		Vector3 position = transform.localPosition;
		position.z -= Random.Range (data.minimalRecoilPushBackPower, data.maximalRecoilPushBackPower);
		transform.localPosition = position;
	}

	void RaycastBullet() {
		RaycastHit hit;
		//GameObject spawnObject2 = Instantiate (bulletObject, null, true);
		//spawnObject2.transform.position = shootPos.transform.position;
		if (Physics.Raycast (shootPos.transform.position, shootPos.transform.forward, out hit)) {
			//spawnObject2.transform.position = hit.point;
			GameObject spawnObject = Instantiate (hitObject, null, true);
			spawnObject.transform.position = hit.point;
			spawnObject.transform.rotation = Quaternion.LookRotation (hit.normal);
			Destroy (spawnObject, 1.0f);
			CharacterDamagablePart characterPart = hit.transform.GetComponent<CharacterDamagablePart> ();
			if (characterPart != null) {
				characterPart.Damage (data.damage, owner);
				if (hit.rigidbody != null) {
					hit.rigidbody.AddForceAtPosition (-hit.normal * 500.0f, hit.point);
				}
			} 
			DestroyableObject destoryableObject = hit.transform.GetComponent<DestroyableObject>();
			if (destoryableObject != null) {
				destoryableObject.Damage (data.damage);
			}
		}
	}

	bool CheckDistance(Vector3 first, Vector3 second, float offset) {
		if(Mathf.Abs(first.magnitude - second.magnitude) < offset){
			return true;
		}
		return false;
	}

	bool MoveAndCheck(Transform transform, Vector3 target, float offset = 0.01f, float speed = 0.1f) {
		transform.position = Vector3.Lerp (
			transform.position, target, speed * Time.deltaTime
		);
		if(CheckDistance(transform.position, target, offset)){
			return true;
		}
		return false;
	}

	bool MoveAndCheckLocal(Transform transform, Vector3 target, float offset = 0.01f, float speed = 0.1f) {
		transform.localPosition = Vector3.Lerp (
			transform.localPosition, target, speed * Time.deltaTime
		);
		if(CheckDistance(transform.localPosition, target, offset)){
			return true;
		}
		return false;
	}

	void ProcessReload() {
		transform.localRotation = Quaternion.Slerp (transform.localRotation, reloadRotation, 5.0f * Time.deltaTime);
		switch (reloadStage) {
		case 0:
			if (MoveAndCheck (leftHand.transform, mag.holdPlace.position, 0.01f, 6.0f)) {
				mag.PlayOutSound ();
				reloadStage++;
			}
			break;
		case 1:
			if (MoveAndCheckLocal (leftHand.transform, leftHandOrigin + (Vector3.left * 2.0f) + (Vector3.down), 0.01f, 6.0f)) {
				reloadStage++;
			}
			mag.transform.position = leftHand.transform.position + (mag.holdPlace.position - mag.transform.position);
			break;
		case 2:
			if (MoveAndCheckLocal (mag.transform, magOrigin, 0.01f, 6.0f)) {
				mag.transform.localPosition = magOrigin;
				mag.PlayInSound ();
				reloadStage++;
			}
			leftHand.transform.position = mag.holdPlace.position;
			break;
		case 3:
			if (MoveAndCheckLocal (leftHand.transform, leftHandOrigin, 0.01f, 7.0f)) {
				if (currentAmmo <= 0) {
					reloadRotation.z = 0.2f;
				}
				leftHand.transform.localPosition = leftHandOrigin;
				reloadStage++;
			}
			break;
		case 4:
			if (currentAmmo <= 0) {
				if (MoveAndCheckLocal (transform, weaponOrigin + (Vector3.left * 0.1f), 0.01f, 10.0f)) {
					reloadStage++;
				}
			} else {
				currentAmmo = reloadAmmo;
				reloading = false;
			}
			break;
		case 5:
			if (
				MoveAndCheck (
					rightHand.transform, 
					bolt.transform.position + (bolt.transform.right * 0.2f), 
					0.01f, 
					13.0f
				)
			) {
				reloadStage++;
			}
			break;
		case 6:
			if (MoveAndCheckLocal (bolt.transform, boltOrigin + (Vector3.back * 0.2f), 0.01f, 15.0f)) {
				bolt.PlaySound ();
				reloadRotation.z = 0.0f;
				reloadStage++;
			}
			rightHand.transform.position = bolt.transform.position + (bolt.transform.right * 0.01f);
			break;
		case 7:
			if (MoveAndCheckLocal (bolt.transform, boltOrigin, 0.01f, 10.0f)) {
				reloadStage++;
			}
			rightHand.transform.localPosition = Vector3.Slerp (rightHand.transform.localPosition, rightHandOrigin, Time.deltaTime * 15.0f);
			break;
		case 8:
			if (MoveAndCheckLocal (rightHand.transform, rightHandOrigin, 0.01f, 7.0f)) {
				reloadStage++;
				rightHand.transform.rotation = rightHandOriginRotation;
			}
			break;
		case 9:
			if (MoveAndCheckLocal (transform, weaponOrigin, 0.01f, 10.0f)) {
				reloadStage++;
				transform.localPosition = weaponOrigin;
			}
			break;
		case 10:
			currentAmmo = reloadAmmo;
			reloading = false;
			break;
		}
	}

	void Start() {
		this.data = this.data.Copy;
		rigidBody = GetComponent<Rigidbody> ();
	}

	private Vector3 previousPosition = Vector3.zero;
	void Update() {
		if (!dropped) {
			if (!reloading) {
				currentRecoil = Vector3.Lerp (currentRecoil, zeroRecoil, Time.deltaTime * 7.0f);
				Quaternion rotation = Quaternion.LookRotation (currentRecoil);
				if (botCase) {
					Quaternion objectRotation = transform.localRotation;
					objectRotation.x += rotation.x / 3.0f;
					objectRotation.y += rotation.y / 3.0f;
					objectRotation.z += rotation.z / 3.0f;
					transform.localRotation = objectRotation;
				} else {
					transform.localRotation = rotation;
				}
			}
			if (inSight && !reloading) {
				if (transform.localPosition != data.sightPosition) {
					transform.localPosition = 
						Vector3.Lerp (transform.localPosition, data.sightPosition, Time.deltaTime * 7.0f);
				}
			} else if (!reloading) {
				if (transform.localPosition != data.idlePosition) {
					transform.localPosition = 
						Vector3.Lerp (transform.localPosition, data.idlePosition, Time.deltaTime * 10.0f);
				}
			}
			if (reloading) {
				ProcessReload ();
			}
			previousPosition = transform.position;
		}
	}

	public void Shoot() {
		if (currentAmmo <= 0) {
			return;
		}
		if (nextTimeShoot <= Time.time && !reloading) {
			for (int counter = 0; counter < shootBulletsCount; counter++) {
				RaycastBullet ();
				Recoil ();
				if (shootBulletsCount > 1) {
					Quaternion rotation = Quaternion.LookRotation (currentRecoil);
					if (botCase) {
						Quaternion objectRotation = transform.localRotation;
						objectRotation.x += rotation.x / 3.0f;
						objectRotation.y += rotation.y / 3.0f;
						objectRotation.z += rotation.z / 3.0f;
						transform.localRotation = objectRotation;
					} else {
						transform.localRotation = rotation;
					}
				}
			}
			if (!botCase) { 
				currentAmmo--;
			}
			shootPos.PlaySound ();
			nextTimeShoot = Time.time + (60.0f / data.fireRate);
		}
	}

	public void Reload() {
		if (inSight || reloading) {
			return;
		}

		if (!botCase && inventory != null) {
			List<Item> items = inventory.FindItemsWithTagInInventory (mag.ammoTag);
			if (items.Count == 0) {
				return;
			}
			int neededToMax = mag.ammoCount - currentAmmo;
			foreach (Item item in items) {
				if (item.currentCount <= neededToMax) {
					neededToMax -= item.currentCount;
					item.RemoveFromInventory ();
				} else {
					item.currentCount -= neededToMax;
					neededToMax = 0;
					break;
				}
			}
			reloadAmmo = mag.ammoCount - neededToMax;
			if (reloadAmmo == 0) {
				return;
			}

			reloading = true;
			reloadCounter = 0.0f;
			boltOrigin = bolt.transform.localPosition;
			rightHandOriginRotation = rightHand.transform.rotation;
			weaponOrigin = transform.localPosition;
			reloadRotation = transform.localRotation;
			reloadStage = 0;
		} else {
			//reloadAmmo = mag.ammoCount;
			currentAmmo = mag.ammoCount;
			reloading = false;
		}

	}

}
