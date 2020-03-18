using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRenderer : MonoBehaviour {
	public Camera camera;
	public RenderTexture renderTexture;
	public Texture borderTexture;

	public static ItemRenderer instance;

	void Awake() {
		instance = this;
		camera.enabled = false;
	}

	public void RenderMesh(GameObject renderObject, Quaternion objectRotation, float cameraOffsetZ, float cameraOffsetX) {
		bool wasEnabled = renderObject.activeInHierarchy;
		renderObject.SetActive (true);
		Vector3 position = renderObject.transform.position;
		Quaternion rotation = renderObject.transform.rotation;
		renderObject.transform.position = transform.position;
		renderObject.transform.rotation = Quaternion.identity;
		transform.rotation = objectRotation;
		Vector3 cameraLocalPosition = camera.transform.localPosition;
		cameraLocalPosition.x = cameraOffsetX;
		cameraLocalPosition.z = cameraOffsetZ;
		camera.transform.localPosition = cameraLocalPosition;
		camera.Render ();
		transform.rotation = Quaternion.identity;
		cameraLocalPosition.z = 0.0f;
		camera.transform.localPosition = cameraLocalPosition;
		renderObject.transform.position = position;
		renderObject.transform.rotation = rotation;
		renderObject.SetActive (wasEnabled);
	}
}