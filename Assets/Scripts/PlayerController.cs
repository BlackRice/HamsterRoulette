using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	private Hamster _hamster;
	public Hamster hamster {
		get {
			if (!_hamster) {
				_hamster = gameObject.GetComponent<Hamster>();
			}
			return _hamster;
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Z)) {
			hamster.DoPrimaryAttack(Game.current.hamsters[1]);
		}

		if (Input.GetKeyDown(KeyCode.X)) {
			hamster.DoSecondaryAttack(Game.current.hamsters[1]);
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			hamster.DoSuperAttack(Game.current.hamsters[1]);
		}
	}
}
