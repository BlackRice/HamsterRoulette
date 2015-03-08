using UnityEngine;
using System.Collections;

public class Hamster : MonoBehaviour {
	public float hp = 100;
	public float mp = 0;

	public float desiredPositionOnWheel = 180;

	public float velocity = 0;
	public float velocityDrag = 1.0f;

	public Hamster target;

	[SerializeField] //Hao - why necessary ?
	private float _distanceFromCenter = 1;
	public float distanceFromCenter {
		get {
			return _distanceFromCenter;
		}
		set {
			_distanceFromCenter = value;
			UpdatePosition();
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

	private Attack primaryAttack;
	private Attack secondaryAttack;
	private Attack superAttack;


	void Awake() {
		Transform parent = new GameObject("HamsterParent").transform;
		parent.SetParent(Wheel.current.transform, false);
		transform.SetParent(parent, false);
		transform.localEulerAngles = new Vector3(0, 90, 0);
		transform.localScale = Vector3.one;
		UpdatePosition();
		positionOnWheel = desiredPositionOnWheel;

		//Assign test attacks.
		primaryAttack = gameObject.AddComponent<Attacks.Test>();
		secondaryAttack = gameObject.AddComponent<Attacks.Test>();
		superAttack = gameObject.AddComponent<Attacks.Test>();
	}

	void UpdatePosition() {
		transform.localPosition = new Vector3(0, 0, distanceFromCenter);
	}

	void FixedUpdate() {
		float gravityFactor = positionOnWheel-desiredPositionOnWheel;
		gravityFactor = Mathf.PingPong(Mathf.Abs(gravityFactor), 90.0f)*Mathf.Sign(gravityFactor);
		velocity -= gravityFactor*Time.fixedDeltaTime*10.0f;
		velocity /= 1.0f+velocityDrag*Time.fixedDeltaTime;
		positionOnWheel += velocity*Time.fixedDeltaTime;
	}

	public void DoPrimaryAttack(Hamster other) {
		primaryAttack.OnAttack(other);
	}

	public void DoSecondaryAttack(Hamster other) {
		secondaryAttack.OnAttack(other);
	}

	public void DoSuperAttack(Hamster other) {
		superAttack.OnAttack(other);
	}

	public void Damage(float amount, Hamster other) {
		hp -= amount;
		if (hp <= 0) {
			Debug.Log("Dead");
		}
	}

}
