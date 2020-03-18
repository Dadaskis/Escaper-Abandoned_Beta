using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class CharacterSaveData {
	public string faction;
	public bool godMode;
	public bool killed = false;
	public int health = 100;
	public int armor = 0;
}

public class Character : MonoBehaviour {

	public string faction;
	public bool godMode;
	public bool killed = false;

	[SerializeField]
	private int health = 100;
	public int Health {
		get {
			return health;
		}
		set {
			health = value;
			onHealthChange.Invoke (health);
		}
	}

	[SerializeField]
	private int armor = 0;
	public int Armor {
		get {
			return armor;
		}
		set {
			armor = value;
			onArmorChange.Invoke (armor);
		}
	}

	public class HealthChangeEvent : UnityEvent<int> {}
	public HealthChangeEvent onHealthChange = new HealthChangeEvent ();

	public class ArmorChangeEvent : UnityEvent<int> {}
	public ArmorChangeEvent onArmorChange = new ArmorChangeEvent ();

	public class DeathEvent : UnityEvent {}
	public DeathEvent onDeath = new DeathEvent();

	public Ragdollizer ragdoll;

	public Scav scav = null;

	public CharacterSaveData Save() {
		CharacterSaveData data = new CharacterSaveData ();

		data.faction = faction;
		data.godMode = godMode;
		data.killed = killed;
		data.health = health;
		data.armor = armor;

		return data;
	}

	public void Load(CharacterSaveData data) {
		faction = data.faction;
		godMode = data.godMode;
		killed = data.killed;
		Health = data.health;
		Armor = data.armor;
	}

	private bool isPlayer = false;
	private Image damageScreen;

	void Start() {
		CharacterManager.Characters.Add (this);
		if (GetComponent<Player> () != null) {
			isPlayer = true;
			damageScreen = Player.instance.damagePanel;
		}
	}

	private float damageScreenResetSpeed = 5.0f;
	void Update() {
		if (isPlayer) {
			Color color = damageScreen.color;
			color.a = Mathf.Lerp (color.a, 0.0f, Time.deltaTime * damageScreenResetSpeed);
			damageScreen.color = color;
		}
	}

	public void Damage(int count, Character character) {
		if (armor < 0) {
			health -= count;
			if (health < 0) {
				Kill ();
				onDeath.Invoke ();
				return;
			}
			if (isPlayer) {
				Color color = Color.red;
				color.a = 0.3f * (100.0f / ((float)health));
				damageScreen.color = color;
			}
			onHealthChange.Invoke (health);
		} else {
			armor--;
			onArmorChange.Invoke (armor);
			if (isPlayer) {
				Color color = new Color (0.2f, 0.2f, 0.2f);
				color.a = 0.3f * (100.0f / ((float)health));
				damageScreen.color = color;
			}
		}
		if (scav != null && character != null) {
			scav.enemy = character;
			scav.status = ScavStatus.ENEMY_DETECTED;
		}
	}

	public void FallDamage(float speed) {
		Debug.Log (speed);
		if (speed < 20.0f) {
			return;
		}
		int count = (int)((speed / 50.0f) * 100);
		health -= count;
		if (health < 0) {
			Kill ();
			onDeath.Invoke ();
			return;
		}
		if (isPlayer) {
			Color color = Color.red;
			color.a = 0.3f * (100.0f / ((float)health));
			damageScreen.color = color;
		}
		onHealthChange.Invoke (health);
	}

	public void Kill() {
		Debug.Log ("Character killed. Faction: " + faction);
		if (!godMode) {
			killed = true;
			CharacterManager.Characters.Remove (this);
			if (ragdoll != null) {
				ragdoll.EnableRagdoll ();
				//Timer.Create (ragdoll.FreezeRagdoll, "RagdollFreeze" + this.GetHashCode (), 3.0f, 1);
				//Destroy (gameObject, 10.0f);
				/*
				RaycastHit hit;
				if (Physics.Raycast (transform.position, -transform.up, out hit)) {
					Instantiate (boxAfterDeath, hit.point, Quaternion.identity);
				}
				*/
			} else {
				//Destroy (gameObject, 0.3f);
			}
		}
	}

}
