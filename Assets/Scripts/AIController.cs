using UnityEngine;
using System.Collections;

public class AIController : Controller {
	void FixedUpdate() {
		hamster.desiredPositionOnWheel = hamster.target.positionOnWheel+hamster.target.velocity*Time.fixedDeltaTime;
	}
}
