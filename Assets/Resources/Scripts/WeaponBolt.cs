using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBolt : MonoBehaviour {
	public Vector3 origin;

	public AudioSource audioSource;
	public AudioClip sound;

	void Start () {
		origin = transform.localPosition;
	}

	public void PlaySound() {
		audioSource.clip = sound;
		audioSource.PlayOneShot (audioSource.clip);
	}
}
