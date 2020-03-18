using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScavSave {
	public WeaponSaveData weapon;
	public ScavStatus status;
	public Vector3 position;
	public Quaternion rotation;
	public CharacterSaveData character;
	public float minimalRecoilControl = 30.0f;
	public float maximalRecoilControl = 60.0f;
	public float checkEnemyTime = 1.0f;
	public float checkDetectedEnemyTime = 1.0f;
	public float updateSpeed = 0.5f;
	public List<ScavItem> items;
}

public enum ScavStatus : int {
	BASE_WALK,
	RAID_WALK,
	ENEMY_DETECTED
};

[System.Serializable]
public class ScavItem {
	[Range(0, 100)]
	public int chance;
	public GameObject spawnObject;
}

public class Scav : MonoBehaviour {

	public static int count = 0;

	public static int countLimit = 20;

	public NavMeshAgent agent;
	public HandsIKPass handsController;
	public float FOV = 90.0f;
	public Character character;
	public float checkEnemyTime = 1.0f;
	public float checkDetectedEnemyTime = 1.0f;
	public float updateSpeed = 0.5f;
	public GameObject weaponHolder;
	public Animator animator;
	public float minimalRecoilControl = 30.0f;
	public float maximalRecoilControl = 60.0f;
	public Character enemy;
	public ScavStatus status = ScavStatus.RAID_WALK;
	public List<ScavItem> items;
	public List<AudioClip> footSounds;
	public float footDelay;
	public AudioSource audioSource;

	private bool wait = false;
	private Vector3 destination;
	private Weapon weapon;
	private bool droppedWeaponAfterDeath;
	private float nextEnemyCheck = 0.0f;
	private float nextDetectedEnemyCheck = 0.0f;
	private bool goToDetectedEnemy = false;
	private float nextUpdateTime = 0.0f;
	private float nextFoot = 0.0f;

	public ScavSave Save() {
		ScavSave save = new ScavSave ();
		save.character = character.Save ();

		if (character.killed) {
			return save;
		}

		save.weapon = weapon.Save ();
		save.position = transform.position;
		save.rotation = transform.rotation;
		save.status = status;
		save.checkDetectedEnemyTime = checkDetectedEnemyTime;
		save.checkEnemyTime = checkEnemyTime;
		save.items = items;
		save.maximalRecoilControl = maximalRecoilControl;
		save.minimalRecoilControl = minimalRecoilControl;
		save.updateSpeed = updateSpeed;

		return save;
	}

	public void Load(ScavSave save){
		weapon = SaveSystem.instance.LoadWeapon (save.weapon, weaponHolder.transform);
		handsController.manipulator = weapon.gameObject;
		transform.position = save.position;
		transform.rotation = save.rotation;
		status = save.status;
		character.Load (save.character);
		checkDetectedEnemyTime = save.checkDetectedEnemyTime;
		checkEnemyTime = save.checkEnemyTime;
		items = save.items;
		maximalRecoilControl = save.maximalRecoilControl;
		minimalRecoilControl = save.minimalRecoilControl;
		updateSpeed = save.updateSpeed;
	}

	void ResetWait() {
		if (character.killed) {
			return;
		}
		wait = false;
		agent.destination = destination;
	}

	void Wait(float seconds) {
		wait = true;
		agent.destination = this.transform.position;
		Timer.Create (ResetWait, "ScavWait" + this.GetInstanceID(), seconds, 1);
	}

	void Start() {
		character.onDeath.AddListener (CharacterDeath);
		//agent.destination = ScavLogic.Raids [Random.Range (0, ScavLogic.Raids.Count)];
		agent.acceleration = 100.0f;
		agent.speed = 8.0f;
		agent.angularSpeed = 999999.0f;
		count += 1;
	}

	void CheckEnemies() {
		if (nextEnemyCheck < Time.time) {
			foreach (Character anotherCharacter in CharacterManager.Characters) {
				if (anotherCharacter == null) {
					continue;
				}
				if (Vector3.Distance (transform.position, anotherCharacter.transform.position) > 200.0f) {
					continue;
				}
				RaycastHit hit;
				bool hitSomething = Physics.Linecast (transform.position, anotherCharacter.transform.position, out hit);
				if (
					Vector3.Angle (anotherCharacter.transform.position - transform.position, transform.forward) <= FOV &&
					hitSomething &&
					hit.collider.transform.root == anotherCharacter.transform.root &&
					anotherCharacter.faction != character.faction) {
					status = ScavStatus.ENEMY_DETECTED;
					enemy = anotherCharacter;
				} else if(hitSomething){
					CharacterDamagablePart characterPart = hit.transform.GetComponent<CharacterDamagablePart> ();
					if (characterPart != null) {
						if (characterPart.character == anotherCharacter) {
							status = ScavStatus.ENEMY_DETECTED;
							enemy = anotherCharacter;
						}
					}
				}
			}
			nextEnemyCheck = Time.time + checkEnemyTime;
		}
	}

	void CharacterDeath() {
		count -= 1;
		agent.enabled = false;
		Destroy (gameObject, 10.0f);

		if (weapon != null) {
			Rigidbody body = weapon.rigidBody;
			weapon.dropped = true;
			weapon.GetComponent<Item> ().pickable = true;
			if (body != null) {
				body.isKinematic = false;
				body.useGravity = true;
				body.detectCollisions = true;
			}
			weapon.transform.parent = null;

			foreach(ScavItem item in items) {
				if(Random.Range(0, 100) > item.chance){
					try {
						Instantiate (item.spawnObject, weapon.transform.position, Quaternion.identity);
					} catch(System.NullReferenceException ex) {
						// I dont know what to say, seriously, i dont know why is this shit happens
					}
				}
			}
		}

		Destroy (this);
	}

	private bool checkingLastPosition = false;
	void DetectCurrentEnemy() {
		if (nextDetectedEnemyCheck < Time.time) {
			RaycastHit hit;
			bool isHit = Physics.Linecast (transform.position, enemy.transform.position, out hit);
			bool colliderIsEqual = false;
			if (isHit) {
				colliderIsEqual = hit.collider.transform.root == enemy.transform.root;
				if (!colliderIsEqual && !checkingLastPosition) {
					agent.destination = enemy.transform.position + (Random.onUnitSphere * 10.0f); 
					goToDetectedEnemy = true;
					checkingLastPosition = true;
				} else if (colliderIsEqual) {
					goToDetectedEnemy = false;	
					checkingLastPosition = false;
				}
			} else {
				if (!checkingLastPosition) {
					agent.destination = enemy.transform.position;
					goToDetectedEnemy = true;
					checkingLastPosition = true;
				}
			}
			nextDetectedEnemyCheck = Time.time + checkDetectedEnemyTime;
		}
	}

	void ShootToCurrentEnemy(){
		Quaternion newRotation = Quaternion.Slerp(
			Quaternion.LookRotation (enemy.transform.position - transform.position),
			transform.rotation,
			Time.deltaTime * 3.0f
		);
		Quaternion currentRotation = transform.rotation;
		currentRotation.y = newRotation.y;
		transform.rotation = currentRotation;
		weapon.inSight = true;
		weapon.transform.rotation = Quaternion.Slerp (
			weapon.transform.rotation,
			Quaternion.LookRotation (enemy.transform.position - weapon.transform.position),
			Time.deltaTime * Random.Range (minimalRecoilControl, maximalRecoilControl)
		);
		weapon.Shoot ();
	}

	void WaitAndWalkToRaidPoint(){
		status = ScavStatus.RAID_WALK;
		Wait (Random.Range (2.0f, 10.0f));
		//destination = ScavLogic.Raids [Random.Range (0, ScavLogic.Raids.Count)] + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2));
		destination = ScavLogic.instance.GetRaidPoint() + (Random.onUnitSphere * 5.0f);
	}

	void WaitAndWalkToBasePoint(){
		//status = ScavStatus.BASE_WALK;
		Wait (5.0f);
		//destination = ScavLogic.Bases [Random.Range (0, ScavLogic.Bases.Count)] + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2));
	}
		
	void Update () {
		if (Scav.count > Scav.countLimit) {
			Destroy (this.gameObject);
			Scav.count--;
		}

		if (Player.instance != null) {
			if (Vector3.Distance (transform.position, Player.instance.transform.position) > ScavLogic.instance.scavSpawnDistanceToPlayer * 1.2f) {
				Destroy (this.gameObject);
				Scav.count--;
			}
		}

		if (weapon == null) {
			weapon = GetComponentInChildren<Weapon> ();
			if (weapon != null) {
				if (weapon.rigidBody != null) {
					weapon.rigidBody.useGravity = false;
					weapon.rigidBody.isKinematic = true;
					weapon.rigidBody.detectCollisions = false;
				}
				weapon.botCase = true;
				weapon.dropped = false;
				weapon.item.pickable = false;
			}
			return;
		}

		if (character.killed) {
			return;
		}

		if (weapon.currentAmmo <= 0) {
			/*Vector3 localPosition = weapon.transform.localPosition;
				localPosition.z = 0.5f;
				weapon.transform.localPosition = localPosition;*/
			weapon.Reload ();
		}
		animator.SetFloat ("Run", ((agent.velocity.magnitude > 5.0f ? 1.0f : 0.0f)));
		if (agent.velocity.magnitude > 0.0f && nextFoot < Time.time) {
			nextFoot = Time.time + footDelay;
			audioSource.clip = footSounds [Random.Range (0, footSounds.Count)];
			audioSource.PlayOneShot (audioSource.clip);
		}
		CheckEnemies ();
		if (nextUpdateTime < Time.time) {
			nextUpdateTime = Time.time + updateSpeed;
			if (!weapon.reloading) {
				weapon.transform.localRotation = Quaternion.Slerp (
					weapon.transform.localRotation,
					Quaternion.identity,
					Time.deltaTime * Random.Range (minimalRecoilControl, maximalRecoilControl)
				);
			}
			if (status == ScavStatus.ENEMY_DETECTED) {
				if (enemy == null) {
					status = ScavStatus.RAID_WALK;
					return;
				}
				DetectCurrentEnemy ();
				if (goToDetectedEnemy) {
					weapon.inSight = false;
				} else {
					agent.destination = transform.position;
					if (!weapon.reloading) {
						ShootToCurrentEnemy ();
					}
				}
			} else {				
				if (!wait) {
					if (!agent.pathPending && agent.remainingDistance < 5.0f) {
						if (status == ScavStatus.BASE_WALK) {
							WaitAndWalkToRaidPoint ();
						} else if (status == ScavStatus.RAID_WALK) {
							WaitAndWalkToBasePoint ();
						}
					}
				}
			}
		}
	}
}
