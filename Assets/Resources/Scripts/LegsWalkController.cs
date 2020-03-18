using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegsWalkController : MonoBehaviour {

	public LeftLeg leftLeg;
	public BezierCurve leftLegWalk;
	public RightLeg rightLeg;
	public BezierCurve rightLegWalk;

	private Vector3 leftLegOrigin;
	private Vector3 rightLegOrigin;

	// Use this for initialization
	void Start () {
		leftLegOrigin = leftLeg.transform.localPosition;
		rightLegOrigin = rightLeg.transform.localPosition;
	}
		
	public void PlayAnimation (float speed) {
		if (speed > 2.0f) {
			leftLeg.transform.position = leftLegWalk.GetPointAt ((Time.time * speed) % 1.0f);
			rightLeg.transform.position = rightLegWalk.GetPointAt ((Time.time * speed) % 1.0f);
		} else {
			leftLeg.transform.localPosition = Vector3.Slerp (
				leftLeg.transform.localPosition,
				leftLegOrigin,
				Time.fixedTime * 10.0f
			);

			rightLeg.transform.localPosition = Vector3.Slerp (
				rightLeg.transform.localPosition,
				rightLegOrigin,
				Time.fixedTime * 10.0f
			);
		}
	}
}
