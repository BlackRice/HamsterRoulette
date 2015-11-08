using UnityEngine;
using System.Collections;

public class Hamster : MonoBehaviour {
	public float maxHP = 100;
	public float maxMP = 100;
	
	[SerializeField]
	private float _hp = 100;
	public float hp {
		get {
			return _hp;
		}
		set {
			_hp = Mathf.Clamp(value, 0, maxHP);
		}
	}

	[SerializeField]
	private float _mp = 100;
	public float mp {
		get {
			return _mp;
		}
		set {
			_mp = Mathf.Clamp(value, 0, maxMP);
		}
	}

	public float hpBaseRegenRate = 5.0f;
	public float mpBaseRegenRate = 10.0f;

	public float minDistanceFromCenter = 0.2f;
	public float maxDistanceFromCenter = 0.8f;

	public RectTransform positionIndicatorPrefab;

	public ValueModifierManager hpModifierManager { get; private set;}
	public ValueModifierManager mpModifierManager { get; private set;}

	//public float desiredPositionOnWheel = 180;

	public float runSpeed = 0;
	public float runSpeedDrag = 1.0f;
	public float centeringForce = 200;
	public float centeringProjection = 0.05f;

	public float collisionRadius = 1;

	public Hamster target;

	public bool isDead {get; private set;}

	[SerializeField]
	private float _desiredDistanceFromCenter = 0.5f;
	public float desiredDistanceFromCenter
	{
		get
		{
			return _desiredDistanceFromCenter;
		}
		set
		{
			_desiredDistanceFromCenter = Mathf.Clamp(value, minDistanceFromCenter, maxDistanceFromCenter);
		}
	}

	private Rigidbody _rigidbody;
	new public Rigidbody rigidbody
	{
		get
		{
			if (!_rigidbody)
			{
				_rigidbody = gameObject.GetComponent<Rigidbody>();
			}
			return _rigidbody;
		}
	}
	/*
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
	*/
	public Attack primaryAttack;
	public Attack secondaryAttack;
	public Attack superAttack;


	void Awake() {
		hpModifierManager = new ValueModifierManager();
		mpModifierManager = new ValueModifierManager();

		//Transform parent = new GameObject("HamsterParent").transform;
		//parent.SetParent(Wheel.current.transform, false);
		//transform.SetParent(parent, false);
		//transform.localEulerAngles = new Vector3(0, 90, 0);
		//transform.localScale = Vector3.one;
		//transform.localPosition = idleLocalPosition;
		//positionOnWheel = desiredPositionOnWheel;


		primaryAttack.onAttackComplete += OnNormalAttackComplete;
		secondaryAttack.onAttackComplete += OnNormalAttackComplete;
	}

	void FixedUpdate() {
		/*
		float gravityFactor = positionOnWheel-desiredPositionOnWheel;
		gravityFactor = Mathf.PingPong(Mathf.Abs(gravityFactor), 90.0f)*Mathf.Sign(gravityFactor);
		velocity -= gravityFactor*Time.fixedDeltaTime*10.0f;
		velocity /= 1.0f+velocityDrag*Time.fixedDeltaTime;

		positionOnWheel += velocity*Time.fixedDeltaTime;
		*/

		//Vector3 forward = transform.position;//Wheel.current.transform.InverseTransformPoint();
		//Quaternion.AngleAxis(1, Wheel.current.spinningTransform.up);
		//forward = (forward-transform.position).normalized;
		//transform.LookAt(transform.position+forward, Wheel.current.spinningTransform.up);

		Rigidbody wheelRB = Wheel.current.spinningTransform.GetComponent<Rigidbody>();
		Vector3 pointVelocity = wheelRB.GetPointVelocity(transform.position);

		if (pointVelocity.magnitude > Vector3.kEpsilon)
		{
			transform.LookAt(transform.position+pointVelocity, Wheel.current.spinningTransform.up);
		}

		Vector3 projectedPosition = transform.position+rigidbody.velocity*centeringProjection;
		Vector3 projectedPositionDif = projectedPosition-Wheel.current.spinningTransform.position;
		float projectedDistance = projectedPositionDif.magnitude;
		Vector3 projectedDirectionFromWheelCenter = projectedPositionDif.normalized;

		Vector3 centerForce = projectedDirectionFromWheelCenter*(desiredDistanceFromCenter-projectedDistance);
		rigidbody.AddForce(centerForce*centeringForce, ForceMode.Acceleration);

		Vector3 floorPosition = GetFloorPosition();
		Vector3 newPosition = floorPosition;
		transform.position = newPosition;

		Vector3 runningVelocity = rigidbody.velocity+transform.forward*runSpeed;
		Vector3 velocityTransfer = runningVelocity-pointVelocity;
		float hamsterWheelMassRatio = rigidbody.mass/wheelRB.mass;
		wheelRB.AddForceAtPosition(velocityTransfer*0.5f*hamsterWheelMassRatio, transform.position, ForceMode.Acceleration);
		rigidbody.AddForce(-velocityTransfer*0.5f/hamsterWheelMassRatio, ForceMode.Acceleration);

		foreach (var otherHamster in Game.current.hamsters) {
			if (otherHamster == this)
			{
				continue;
			}

			ApplyHamsterCollision(otherHamster);
		}

		Vector3 vel = rigidbody.velocity;
		//vel = pointVelocity;
		vel = transform.InverseTransformDirection(vel);
		vel.y = 0;
		vel = transform.TransformDirection(vel);
		rigidbody.velocity = vel;

		runSpeed /= 1.0f+runSpeedDrag*Time.fixedDeltaTime;

		float newHP = hp;
		newHP += hpBaseRegenRate*hpModifierManager.value*Time.fixedDeltaTime;
		hp = newHP;

		float newMP = mp;
		newMP += mpBaseRegenRate*mpModifierManager.value*Time.fixedDeltaTime;
		mp = newMP;
	}

	void ApplyHamsterCollision(Hamster otherHamster)
	{
		Vector3 positionDif = otherHamster.transform.position-transform.position;
		float distance = positionDif.magnitude;
		Vector3 direction = positionDif.normalized;

		float overlap = 1.0f-Mathf.Clamp01(distance/(collisionRadius+otherHamster.collisionRadius));

		if (overlap <= 0)
		{
			return;
		}

		Vector3 force = direction*overlap*10;
		rigidbody.AddForce(-force*otherHamster.rigidbody.mass);
		otherHamster.rigidbody.AddForce(force*rigidbody.mass);
	}

	Vector3 GetFloorPosition()
	{
		Ray ray = new Ray(transform.position, -Wheel.current.spinningTransform.up);
		ray.origin -= ray.direction*0.5f;
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			return hit.point;
		}

		return ray.origin;
	}

	private void OnNormalAttackComplete(float damageDone)
	{
		superAttack.charge += damageDone;
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

		Game.current.OnHamsterDie(this);
	}

	void OnDestroy()
	{
		//Destroy(transform.parent.gameObject);
	}
}
