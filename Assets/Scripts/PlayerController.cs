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

		if (Input.GetKeyDown(KeyCode.Space))
		{
			hamster.runSpeed += 5;
		}

		//TODO: use actual tilt input.
		float tilt = 1.0f-(Input.mousePosition.y/Screen.height);
		hamster.desiredDistanceFromCenter = tilt;
	}
}
