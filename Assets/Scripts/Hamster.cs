using UnityEngine;
using System.Collections;

public class Hamster : MonoBehaviour {
	public float hp = 100;
	public float mp = 0;

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
		Transform parent = new GameObject("HamsterParent").transform;
		parent.SetParent(Wheel.current.transform, false);
		transform.SetParent(parent, false);
		transform.localEulerAngles = new Vector3(0, 90, 0);
		transform.localScale = Vector3.one;
		transform.localPosition = idleLocalPosition;
		positionOnWheel = desiredPositionOnWheel;

		//Assign test attacks.
		//primaryAttack = gameObject.AddComponent<Attacks.Test>();
		//secondaryAttack = gameObject.AddComponent<Attacks.Test>();
		//superAttack = gameObject.AddComponent<Attacks.Test>();
	}

	void FixedUpdate() {
		float gravityFactor = positionOnWheel-desiredPositionOnWheel;
		gravityFactor = Mathf.PingPong(Mathf.Abs(gravityFactor), 90.0f)*Mathf.Sign(gravityFactor);
		velocity -= gravityFactor*Time.fixedDeltaTime*10.0f;
		velocity /= 1.0f+velocityDrag*Time.fixedDeltaTime;
		positionOnWheel += velocity*Time.fixedDeltaTime;
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
