using UnityEngine;
using System.Collections;

public class PlayerController : Controller {


	void Update() {
		if (Input.GetKeyDown(KeyCode.Z)) {
			hamster.DoPrimaryAttack(hamster.target);
		}

		if (Input.GetKeyDown(KeyCode.X)) {
			hamster.DoSecondaryAttack(hamster.target);
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			hamster.DoSuperAttack(hamster.target);
		}
	}

	void FixedUpdate() {
		float tilt = (Input.mousePosition.x/Screen.width)*2.0f-1.0f;
		hamster.desiredPositionOnWheel = 180.0f-tilt*90;
		hamster.desiredPositionOnWheel = Mathf.Clamp(hamster.desiredPositionOnWheel, 180-90, 180+90);
	}
}
