using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertialMoving : MonoBehaviour {

	Vector3 previousPosition;
	Quaternion previousRotation;
	Transform myTransform;
	Vector3 origin;
	Quaternion originRotation;

	public float power = 1.0f;
	public float rotationPower = 0.3f;

	void Start(){
		myTransform = transform;
		origin = myTransform.localPosition;
		originRotation = myTransform.localRotation;
		previousPosition = myTransform.position;
	}

	void FixedUpdate () {
		myTransform.position = previousPosition;
		//myTransform.localPosition += ((origin - myTransform.localPosition) * power);
		myTransform.localPosition = Vector3.Slerp(myTransform.localPosition, origin, power);
		myTransform.rotation = previousRotation;
		myTransform.localRotation = Quaternion.Slerp (myTransform.localRotation, originRotation, rotationPower);
		previousRotation = myTransform.rotation;
		previousPosition = myTransform.position;
	}
}
