using UnityEngine;
using System.Collections;

public class Attack:MonoBehaviour {
	public float damage = 25;

	private Hamster _hamster;
	public Hamster hamster {
		get {
			if (!_hamster) {
				_hamster = gameObject.GetComponent<Hamster>();
			}
			return _hamster;
		}
	}

	public void DoAttack(Hamster other) {
		StartCoroutine(OnAttack(other));
	}

	protected virtual IEnumerator OnAttack(Hamster other) {
		yield break;
	}
}
