using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMag : MonoBehaviour {

	public Vector3 origin;
	public AudioSource audioSource;
	public AudioClip outSound;
	public AudioClip inSound;
	public Transform holdPlace;
	public int ammoCount = 0;
	public string ammoTag;

	void Start () {
		origin = transform.localPosition;
	}

	public void PlayInSound() {
		audioSource.clip = inSound;
		audioSource.PlayOneShot (audioSource.clip);
	}

	public void PlayOutSound() {
		audioSource.clip = outSound;
		audioSource.PlayOneShot (audioSource.clip);
	}
}
