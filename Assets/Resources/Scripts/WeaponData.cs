using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponDataSerializable { 
	public Vector3 idlePosition;
	public Quaternion idleRotation;
	public Vector3 sightPosition; 
	public Quaternion sightRotation;
	public float downRecoil = 0.0f;
	public float upRecoil = 0.0f;
	public float leftRecoil = 0.0f;
	public float rightRecoil = 0.0f;
	public float minimalRecoilPushBackPower = 0.0f;
	public float maximalRecoilPushBackPower = 0.0f;
	public float fireRate = 0.0f;
	public int damage = 0;

	public void CopyFrom(WeaponData data) {
		idlePosition = data.idlePosition;
		idleRotation = data.idleRotation;
		sightPosition = data.sightPosition;
		sightRotation = data.sightRotation;
		downRecoil = data.downRecoil;
		upRecoil = data.upRecoil;
		leftRecoil = data.leftRecoil;
		rightRecoil = data.rightRecoil;
		minimalRecoilPushBackPower = data.minimalRecoilPushBackPower;
		maximalRecoilPushBackPower = data.maximalRecoilPushBackPower;
		fireRate = data.fireRate;
		damage = data.damage;
	}

	public WeaponData GetScriptableObject() {
		WeaponData data = ScriptableObject.CreateInstance<WeaponData>();
		data.idlePosition = idlePosition;
		data.idleRotation = idleRotation;
		data.sightPosition = sightPosition;
		data.sightRotation = sightRotation;
		//data.weaponObject = weaponObject;
		data.downRecoil = downRecoil;
		data.upRecoil = upRecoil;
		data.leftRecoil = leftRecoil;
		data.rightRecoil = rightRecoil;
		data.minimalRecoilPushBackPower = minimalRecoilPushBackPower;
		data.maximalRecoilPushBackPower = maximalRecoilPushBackPower;
		data.fireRate = fireRate;
		data.damage = damage;
		return data;
	}
}

[CreateAssetMenu(fileName = "Weapon data", menuName = "New weapon data")]
[System.Serializable]
public class WeaponData : ScriptableObject {
	public Vector3 idlePosition;
	public Quaternion idleRotation;
	public Vector3 sightPosition; 
	public Quaternion sightRotation;
	public float downRecoil = 0.0f;
	public float upRecoil = 0.0f;
	public float leftRecoil = 0.0f;
	public float rightRecoil = 0.0f;
	public float minimalRecoilPushBackPower = 0.0f;
	public float maximalRecoilPushBackPower = 0.0f;
	public float fireRate = 0.0f;
	public int damage = 0;

	public WeaponData Copy {
		get {
			WeaponData data = ScriptableObject.CreateInstance<WeaponData>();
			data.idlePosition = idlePosition;
			data.idleRotation = idleRotation;
			data.sightPosition = sightPosition;
			data.sightRotation = sightRotation;
			//data.weaponObject = weaponObject;
			data.downRecoil = downRecoil;
			data.upRecoil = upRecoil;
			data.leftRecoil = leftRecoil;
			data.rightRecoil = rightRecoil;
			data.minimalRecoilPushBackPower = minimalRecoilPushBackPower;
			data.maximalRecoilPushBackPower = maximalRecoilPushBackPower;
			data.fireRate = fireRate;
			data.damage = damage;
			return data;
		}
	}
}
