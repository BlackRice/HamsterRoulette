using UnityEngine;
using System.Collections;

public class PlayerController : Controller {
	void Update() {
		if (hamster.isDead)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Z)) {
			hamster.DoPrimaryAttack(hamster.target);
		}

		if (Input.GetKeyDown(KeyCode.X)) {
			hamster.DoSecondaryAttack(hamster.target);
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			hamster.DoSuperAttack(hamster.target);
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			hamster.runDirection = -1;
			hamster.runSpeed += 1;
		}

		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			hamster.runDirection = 1;
			hamster.runSpeed += 1;
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			hamster.desiredDistanceFromCenter -= 1.0f*Time.deltaTime;
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			hamster.desiredDistanceFromCenter += 1.0f*Time.deltaTime;
		}

		//TODO: use actual tilt input.
		//float tilt = 1.0f-(Input.mousePosition.y/Screen.height);
		//hamster.desiredDistanceFromCenter = tilt;
	}
}
