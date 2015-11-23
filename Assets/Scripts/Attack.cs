using UnityEngine;
using System.Collections;

public class Attack:MonoBehaviour {
	public delegate void OnAttackComplete(float damageDone);
	public event OnAttackComplete onAttackComplete;

	public float damage = 25;
	public float mpCost = 10;
	public float coolDownTime = 0;
	public float charge = 0;
	public float requiredCharge = 0;
	public float maxRadiusFalloff = 0;

	public float damageIncrement = 5;

	public float lastAttackTime { get; private set; }
	public float coolDown {
		get {
			if (coolDownTime <= 0)
			{
				return 1;
			}
			return Mathf.Clamp01((Time.timeSinceLevelLoad-lastAttackTime)/coolDownTime);
		}
	}

	public float chargeRatio {
		get {
			if (requiredCharge <= 0)
			{
				return 1;
			}
			return Mathf.Clamp01(charge/requiredCharge);
		}
	}

	public bool isUsable
	{
		get
		{
			return coolDown >= 1 && chargeRatio >= 1 && hamster.mp >= mpCost;
		}
	}

	private Hamster _hamster;
	public Hamster hamster {
		get {
			if (!_hamster) {
				_hamster = gameObject.GetComponent<Hamster>();
			}
			return _hamster;
		}
	}

	public Attack()
	{
		lastAttackTime = Mathf.NegativeInfinity;
	}

	public float GetDamageModifier(Hamster other)
	{
		float damageModifier = 1;

		if (maxRadiusFalloff > 0) {
			float distance = Vector3.Distance(transform.position, other.transform.position);

			//Subtract both collision radiuses so it uses the distance between collision spheres.
			distance -= hamster.collisionRadius+other.collisionRadius;
			distance = Mathf.Max(distance, 0);

			float ratio = 1.0f-Mathf.Clamp01(distance/maxRadiusFalloff);
			damageModifier = ratio;
		}
		return damageModifier;
	}

	public float GetDamage(Hamster other) {
		float actualDamage = damage*GetDamageModifier(other);
		actualDamage = Mathf.Round(actualDamage/damageIncrement)*damageIncrement;
		return actualDamage;
	}

	public void DoAttack(Hamster other) {
		if (!isUsable) {
			return;
		}

		lastAttackTime = Time.timeSinceLevelLoad;
		hamster.mp -= mpCost;
		charge = 0;
		StartCoroutine(ProxyOnAttack(other));
	}

	private IEnumerator ProxyOnAttack(Hamster other)
	{
		yield return StartCoroutine(OnAttack(other));

		if (onAttackComplete != null)
		{
			float d = GetDamage(other);
			onAttackComplete(d);
		}
	}

	protected virtual IEnumerator OnAttack(Hamster other) {
		yield break;
	}
}
