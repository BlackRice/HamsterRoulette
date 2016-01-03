using UnityEngine;
using System.Collections;

public class Hamster : MonoBehaviour {
	public class SpinningStateInfo
	{
		public float angleOnWheel;
		public SpinningStateInfo(Hamster hamster)
		{
			angleOnWheel = Hamster.CalculateAngle(Wheel.current.spinningTransform, hamster.transform.position);
		}
	}

	public float maxHP = 100;
	public float maxMP = 100;

	public DamageIndicator damageIndicatorTemplate;

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

	public float angleOnWheel
	{
		get
		{
			return CalculateAngle(Wheel.current.transform, transform.position);
		}
	}

	public float angularVelocityOnWheel
	{
		get
		{
			float angle = angleOnWheel;
			float projectedAngle = CalculateAngle(Wheel.current.transform, transform.position+rigidbody.velocity);
			return Mathf.DeltaAngle(angle, projectedAngle);
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

	private int _runDirection = 1;
	public int runDirection
	{
		get
		{
			return _runDirection;
		}
		set
		{
			int previous = _runDirection;
			_runDirection = (int)Mathf.Sign(value);
			if (previous != _runDirection)
			{
				runSpeed = 0;
			}
		}
	}

	public Vector3 forwardDirection
	{
		get
		{
			Vector3 directionFromWheel = (transform.position-Wheel.current.transform.position).normalized;
			Vector3 forward = Vector3.Cross(Wheel.current.transform.up, directionFromWheel)*runDirection;
			return forward;
		}
	}

	public Attack primaryAttack;
	public Attack secondaryAttack;
	public Attack superAttack;

	public SpinningStateInfo spinningStateInfo {get; private set; }

	public void StartSpinning()
	{
		spinningStateInfo = new SpinningStateInfo(this);
		rigidbody.velocity = Vector3.zero;
		rigidbody.isKinematic = true;
	}

	public void EndSpinning()
	{
		spinningStateInfo = null;
		rigidbody.isKinematic = false;
	}

	void Awake() {
		hpModifierManager = new ValueModifierManager();
		mpModifierManager = new ValueModifierManager();

		primaryAttack.onAttackComplete += OnNormalAttackComplete;
		secondaryAttack.onAttackComplete += OnNormalAttackComplete;
	}

	void FixedUpdate()
	{
		if (spinningStateInfo != null)
		{
			SpinningStateUpdate();
		}
		else
		{
			RunningStateUpdate();
		}
	}

	void SpinningStateUpdate()
	{
		Vector3 localSpinningPosition = Quaternion.Euler(0, spinningStateInfo.angleOnWheel, 0)*Vector3.forward*maxDistanceFromCenter;
		transform.position = Wheel.current.spinningTransform.TransformPoint(localSpinningPosition);
		transform.LookAt(transform.position+forwardDirection, Wheel.current.spinningTransform.up);
	}

	void RunningStateUpdate() {
		Rigidbody wheelRB = Wheel.current.spinningTransform.GetComponent<Rigidbody>();
		Vector3 pointVelocity = wheelRB.GetPointVelocity(transform.position);

		if (pointVelocity.magnitude > Vector3.kEpsilon)
		{
			transform.LookAt(transform.position+forwardDirection, Wheel.current.spinningTransform.up);
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
		//wheelRB.AddForceAtPosition(velocityTransfer*0.5f*hamsterWheelMassRatio, transform.position, ForceMode.VelocityChange);
		//rigidbody.AddForce(-velocityTransfer*0.5f/hamsterWheelMassRatio, ForceMode.VelocityChange);
		wheelRB.AddForceAtPosition(velocityTransfer*5.0f*hamsterWheelMassRatio, transform.position, ForceMode.Acceleration);
		rigidbody.AddForce(-velocityTransfer*5.0f/hamsterWheelMassRatio, ForceMode.Acceleration);

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

		if (!isDead)
		{
			float newHP = hp;
			newHP += hpBaseRegenRate*hpModifierManager.value*Time.fixedDeltaTime;
			hp = newHP;

			float newMP = mp;
			newMP += mpBaseRegenRate*mpModifierManager.value*Time.fixedDeltaTime;
			mp = newMP;
		}
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

	static private float CalculateAngle(Transform t, Vector3 position)
	{
		Vector3 localPosition = t.InverseTransformPoint(position);
		localPosition.y = 0;
		if (localPosition == Vector3.zero)
		{
			return 0;
		}
		return Quaternion.LookRotation(localPosition).eulerAngles.y;
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

		if (damageIndicatorTemplate)
		{
			DamageIndicator damageIndicator = (DamageIndicator)Instantiate(damageIndicatorTemplate, transform.position+new Vector3(0, 0.35f, 0), Quaternion.identity);
			damageIndicator.damage = Mathf.RoundToInt(amount);
		}

		if (!isDead && hp <= 0) {
			Kill();
		}
	}

	public void Kill() {
		if (isDead) return;
		isDead = true;
		Game.current.OnHamsterDie(this);
	}
}
