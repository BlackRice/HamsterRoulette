using UnityEngine;
using System.Collections;

public class Wheel:MonoBehaviour {
	static private Wheel _current;
	static public Wheel current {
		get {
			if (!_current) {
				_current = FindObjectOfType<Wheel>();
			}
			return _current;
		}
	}

	public Transform spinningTransform;
	public float spinRate = 10000;

	void Update() {
		spinningTransform.Rotate(0, spinRate*Time.deltaTime, 0);
	}
}
