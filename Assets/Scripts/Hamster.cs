using UnityEngine;
using System.Collections;

public class Hamster : MonoBehaviour {
	[SerializeField]
	public float _hp = 100;
	public float hp {
		get {
			return _hp;
		}
		set {
			_hp = Mathf.Clamp(value, 0, 100);
		}
	}

	[SerializeField]
	public float _mp = 100;
	public float mp {
		get {
			return _mp;
		}
		set {
			_mp = Mathf.Clamp(value, 0, 100);
		}
	}

	public float hpBaseRegenRate = 1.0f;
	public float mpBaseRegenRate = 1.0f;

	public ValueModifierManager hpModifierManager { get; private set;}
	public ValueModifierManager mpModifierManager { get; private set;}

	public float desiredPositionOnWheel = 180;

	public float velocity = 0;
	public float velocityDrag = 1.0f;

	public Hamster target;

	public bool isDead {get; private set;}

	[SerializeField] //Hao - why necessary ?
	private float _distanceFromCenter = 1;
	public float distanceFromCenter {
		get {
			return _distanceFromCenter;
		}
		set {
			_distanceFromCenter = value;
			transform.localPosition = idleLocalPosition;
		}
	}

	public Vector3 idleLocalPosition {
		get {
			return new Vector3(0, 0, distanceFromCenter);
		}
	}

	public float positionOnWheel {
		get {
			return transform.parent.localEulerAngles.y;
		}
		set {
			value = Mathf.Repeat(value, 360);
			Vector3 angles = transform.parent.localEulerAngles;
			angles.y = value;
			transform.parent.localEulerAngles = angles;
		}
	}

	public Attack primaryAttack;
	public Attack secondaryAttack;
	public Attack superAttack;


	void Awake() {
		hpModifierManager = new ValueModifierManager();
		mpModifierManager = new ValueModifierManager();

		Transform parent = new GameObject("HamsterParent").transform;
		parent.SetParent(Wheel.current.transform, false);
		transform.SetParent(parent, false);
		transform.localEulerAngles = new Vector3(0, 90, 0);
		transform.localScale = Vector3.one;
		transform.localPosition = idleLocalPosition;
		positionOnWheel = desiredPositionOnWheel;
	}

	void FixedUpdate() {
		float gravityFactor = positionOnWheel-desiredPositionOnWheel;
		gravityFactor = Mathf.PingPong(Mathf.Abs(gravityFactor), 90.0f)*Mathf.Sign(gravityFactor);
		velocity -= gravityFactor*Time.fixedDeltaTime*10.0f;
		velocity /= 1.0f+velocityDrag*Time.fixedDeltaTime;
		positionOnWheel += velocity*Time.fixedDeltaTime;

		float newHP = hp;
		newHP += hpBaseRegenRate*hpModifierManager.value*Time.fixedDeltaTime;
		hp = newHP;

		float newMP = mp;
		newMP += mpBaseRegenRate*mpModifierManager.value*Time.fixedDeltaTime;
		mp = newMP;
	}

	public void DoPrimaryAttack(Hamster other) {
		primaryAttack.DoAttack(other);
	}

	public void DoSecondaryAttack(Hamster other) {
		secondaryAttack.DoAttack(other);
	}

	public void DoSuperAttack(Hamster other) {
		superAttack.DoAttack(other);
	}

	public void Damage(float amount, Hamster other) {
		hp -= amount;
		if (!isDead && hp <= 0) {
			Kill();
		}
	}

	public void Kill() {
		if (isDead) return;

		Destroy(transform.parent.gameObject);
		Debug.Log("Dead");
	}

}
