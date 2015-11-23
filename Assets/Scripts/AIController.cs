using UnityEngine;
using System.Collections;

public class AIController : Controller {
	public enum Tactic {Normal, Attack, Heal}
	public Tactic tactic = Tactic.Normal;

	public float mpConservationFactor = 1;
	public float minAttackRate = 1.0f;
	public float maxAttackRate = 5.0f;

	private float nextAttackAttempt = 0;
	private float lastTacticUpdateTime = Mathf.NegativeInfinity;

	void FixedUpdate() {
		if (Time.timeSinceLevelLoad-lastTacticUpdateTime > 3)
		{
			UpdateTactic();
		}

		ProcessMovement();

		ProcessAttacking();

		//hamster.desiredPositionOnWheel = hamster.target.positionOnWheel+hamster.target.velocity*Time.fixedDeltaTime;
	}

	void UpdateTactic()
	{
		float hpRatio = hamster.hp/hamster.target.hp;

		tactic = Tactic.Normal;

		if (Random.value > 0.5f)
		{
			tactic = Tactic.Attack;
			if (Random.value > 0.5f)
			{
				tactic = Tactic.Heal;
			}
		}

		if (hpRatio*(1.0f+Random.value*0.2f) > 1)
		{
			tactic = Tactic.Attack;
		}
		else
		{
			if (Random.value > 0.5f)
			{
				tactic = Tactic.Heal;
			}
		}

		lastTacticUpdateTime = Time.timeSinceLevelLoad;
	}

	private void ProcessMovement()
	{
		float angleDifference = Mathf.DeltaAngle(hamster.angleOnWheel, hamster.target.angleOnWheel);

		float moveTowardsFactor = 0;
		if (tactic == Tactic.Attack)
		{
			moveTowardsFactor = 1;
		}
		else if (tactic == Tactic.Heal)
		{
			moveTowardsFactor = -1;
		}

		float speedModifier = (moveTowardsFactor*Mathf.Sign(-angleDifference))+1;

		hamster.runSpeed += 5.0f*speedModifier*Time.fixedDeltaTime;

	}

	private void ProcessAttacking()
	{
		if (Time.timeSinceLevelLoad < nextAttackAttempt)
		{
			return;
		}

		Attack[] attacks = new Attack[]
		{
			hamster.primaryAttack,
			hamster.secondaryAttack,
			hamster.superAttack
		};

		float[] attackWeights = new float[attacks.Length];

		for (int i = 0; i < attacks.Length; i++)
		{
			Attack attack = attacks[i];
			if (!attack.isUsable)
			{
				continue;
			}

			float damage = attack.GetDamage(hamster.target);
			attackWeights[i] = damage;
		}

		Attack bestAttack = null;
		float bestAttackWeight = 0;

		for (int i = 0; i < attacks.Length; i++)
		{
			if (attackWeights[i] > bestAttackWeight)
			{
				bestAttackWeight = attackWeights[i];
				bestAttack = attacks[i];
			}
		}

		if (bestAttack)
		{
			bestAttack.DoAttack(hamster.target);
			nextAttackAttempt = Time.timeSinceLevelLoad+Random.Range(minAttackRate, maxAttackRate);
		}
	}
}
