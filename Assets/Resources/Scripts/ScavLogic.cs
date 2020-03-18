﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScavLogic : MonoBehaviour {

	private List<Vector3> spawnPoints = new List<Vector3>();
	private List<Vector3> basePoints = new List<Vector3>();
	private List<Vector3> raidPoints = new List<Vector3>();

	public static ScavLogic instance;

	public float updateRaidPointDelay = 60.0f;
	public float raidPointPlayerDistance = 20.0f;
	public float scavSpawnDistanceToPlayer = 200.0f;
	public float spawnScavDelay = 100.0f;

	public List<Scav> scavs;

	private float raidPointTimer = 1000000.0f;
	private float spawnScavTimer = 1000000.0f;

	void Awake() {
		instance = this;
	}

	public static List<Vector3> Spawns {
		get {
			return instance.spawnPoints;
		}
	}

	public static List<Vector3> Bases {
		get {
			return instance.basePoints;
		}
	}

	public static List<Vector3> Raids {
		get {
			return instance.raidPoints;
		}
	}

	private Vector3 raidPoint = new Vector3 ();

	Vector3 GetRandomPositionOnNavMesh(float radius) {
		Vector3 randomDirection = Random.onUnitSphere * radius;
		if (Player.instance != null) {
			randomDirection += Player.instance.transform.position;
		}
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, radius, 1);
		return hit.position;
	}

	public Vector3 GetRaidPoint() {
		return raidPoint;
	}

	void Update() {
		if (Player.instance == null) {
			return;
		}
		raidPointTimer += Time.deltaTime;
		spawnScavTimer += Time.deltaTime;

		if (raidPointTimer > updateRaidPointDelay) {
			raidPointTimer = 0.0f;
			raidPoint = GetRandomPositionOnNavMesh (raidPointPlayerDistance);
		}
			
		if (spawnScavTimer > spawnScavDelay) {
			Vector3 playerPosition = Player.instance.transform.position;
			if (Scav.count < Scav.countLimit) {
				Vector3 spawnPosition;// = GetRandomPositionOnNavMesh (scavSpawnDistanceToPlayer);
				for (int counter = 0; counter < Scav.countLimit - Scav.count; counter++) {
					if (Scav.count > Scav.countLimit) {
						break;
					}
					spawnPosition = GetRandomPositionOnNavMesh (scavSpawnDistanceToPlayer);
					while (spawnPosition.y > playerPosition.y) {
						spawnPosition = GetRandomPositionOnNavMesh (scavSpawnDistanceToPlayer);
						if (Random.Range (0, 100) > 90) {
							break;
						}
					}

					spawnScavTimer = 0.0f;
					if (spawnPosition.magnitude < 10000.0f) {
						Instantiate (scavs [Random.Range (0, scavs.Count - 1)], spawnPosition, Quaternion.identity);
					}
				}
			}
		}
	}

}