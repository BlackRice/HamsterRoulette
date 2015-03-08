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
		float tilt = Input.GetAxis("Horizontal");
		
		//hamster.velocity -= tilt*360.0f*Time.fixedDeltaTime;
		hamster.desiredPositionOnWheel -= tilt*360.0f*Time.fixedDeltaTime;
		hamster.desiredPositionOnWheel = Mathf.Clamp(hamster.desiredPositionOnWheel, 180-90, 180+90);
	}
}
