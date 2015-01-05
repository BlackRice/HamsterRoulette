using UnityEngine;
using System.Collections;

public class Attack:MonoBehaviour {
	private Hamster _hamster;
	public Hamster hamster {
		get {
			if (!_hamster) {
				_hamster = gameObject.GetComponent<Hamster>();
			}
			return _hamster;
		}
	}

	public virtual void OnAttack(Hamster other) {

	}
}
