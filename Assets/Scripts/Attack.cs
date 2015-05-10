using UnityEngine;
using System.Collections;

public class Attack:MonoBehaviour {
	public float damage = 25;
	public float mpCost = 10;

	public float alignmentOffset = 5;
	public float alignmentFalloff = 10;

	public float damageIncrement = 5;

	private Hamster _hamster;
	public Hamster hamster {
		get {
			if (!_hamster) {
				_hamster = gameObject.GetComponent<Hamster>();
			}
			return _hamster;
		}
	}

	public float GetAlignmentFactor(Hamster other) {
		float alignmentFactor = Mathf.Abs(Mathf.DeltaAngle(hamster.positionOnWheel, other.positionOnWheel));
		alignmentFactor -= alignmentOffset;
		alignmentFactor /= alignmentFalloff;
		alignmentFactor = 1.0f-Mathf.Clamp01(alignmentFactor);

		return alignmentFactor;
	}

	public float GetDamage(Hamster other) {
		float actualDamage = damage*GetAlignmentFactor(other);
		actualDamage = Mathf.Round(actualDamage/damageIncrement)*damageIncrement;
		return actualDamage;
	}

	public void DoAttack(Hamster other) {
		hamster.mp -= mpCost;
		StartCoroutine(OnAttack(other));
	}

	protected virtual IEnumerator OnAttack(Hamster other) {
		yield break;
	}
}
