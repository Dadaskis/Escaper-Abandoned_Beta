using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class StoryWindowCursorChanger : MonoBehaviour {

	void Update () {
		FirstPersonController controller = Player.instance.GetComponent<FirstPersonController> ();
		if (controller != null) {
			controller.enableMouseLook = false;
			controller.mouseLook.SetCursorLock (false);
		} else {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
