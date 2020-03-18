using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidIKPass : MonoBehaviour {
	public Animator animator;
	public GameObject manipulator;
	private LeftHand leftHandObject;
	private RightHand rightHandObject;

	void OnAnimatorIK() {
		if (manipulator == null) {
			return;
		}

		if (leftHandObject == null || rightHandObject == null) {
			LeftHand[] leftHandObjects = manipulator.GetComponentsInChildren<LeftHand> ();
			leftHandObject = leftHandObjects [leftHandObjects.Length - 1];
			RightHand[] rightHandObjects = manipulator.GetComponentsInChildren<RightHand> ();
			rightHandObject = rightHandObjects [rightHandObjects.Length - 1];
			if (leftHandObject == null || rightHandObject == null) {
				Debug.LogWarning ("Not found hands");
				return;
			}
		}

		Transform leftHand = leftHandObject.transform;
		Transform rightHand = rightHandObject.transform;

		//animator.SetLookAtWeight (1.0f);
		//animator.SetLookAtPosition (transform.position);

		animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1.0f);
		animator.SetIKRotationWeight (AvatarIKGoal.LeftHand, 1.0f);
		animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHand.position);
		animator.SetIKRotation (AvatarIKGoal.LeftHand, leftHand.rotation);

		animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 1.0f);
		animator.SetIKRotationWeight (AvatarIKGoal.RightHand, 1.0f);
		animator.SetIKPosition (AvatarIKGoal.RightHand, rightHand.position);
		animator.SetIKRotation (AvatarIKGoal.RightHand, rightHand.rotation);
	}
}
