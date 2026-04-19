using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class LevelRoadEnd : MonoBehaviour {

	public string name = "";

	public bool overridePlayerRotation = true;
	public bool resetVelocity = true;

	void Start () {
		Debug.Log (name);
		LevelRoadEndManager.AddLevelRoadEnd (this);
	}

	// NEW: Draw arrow in editor
	void OnDrawGizmos() {
		// Draw the existing sphere (keep the original)
		Gizmos.color = new Color(255, 255, 50, 10);
		Gizmos.DrawWireSphere (transform.position, 0.3f);

		// Draw arrow for rotation
		Gizmos.color = Color.cyan;
		Vector3 arrowDirection = transform.forward;
		Vector3 arrowStart = transform.position;
		Vector3 arrowEnd = arrowStart + arrowDirection * 0.8f;

		// Draw line
		Gizmos.DrawLine(arrowStart, arrowEnd);

		// Draw arrow head
		Vector3 right = Quaternion.LookRotation(arrowDirection) * Quaternion.Euler(0, 135, 0) * Vector3.forward;
		Vector3 left = Quaternion.LookRotation(arrowDirection) * Quaternion.Euler(0, 225, 0) * Vector3.forward;
		Gizmos.DrawLine(arrowEnd, arrowEnd + right * 0.2f);
		Gizmos.DrawLine(arrowEnd, arrowEnd + left * 0.2f);

		// Draw a small circle at the base to show origin
		Gizmos.DrawWireSphere(arrowStart, 0.05f);
	}

	// Apply rotation and reset velocity
	public void ApplyToPlayer(Player player) {
		if (overridePlayerRotation) {
			// Set player's camera rotation (Y axis only for horizontal, or full rotation?)
			// For first-person, you usually want to preserve vertical look but set horizontal facing
			Transform playerCamera = player.camera.transform;
			Vector3 currentEuler = playerCamera.eulerAngles;
			currentEuler.y = transform.eulerAngles.y;
			playerCamera.eulerAngles = currentEuler;
		}

		if (resetVelocity) {
			// Reset Rigidbody velocity if the player has one
			Rigidbody rb = player.GetComponent<Rigidbody>();
			if (rb != null) {
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}

			FirstPersonController controller = player.GetComponent<FirstPersonController> ();
			if (controller != null) {
				controller.SetStandLookAngle (transform.eulerAngles.y);
				controller.PushController (transform.forward);
			}
		}
	}
}