using UnityEngine;
using System.Collections;

public class Wheel:MonoBehaviour {
	public Transform spinningTransform;
	public float spinRate = 10000;

	void Update() {
		spinningTransform.Rotate(0, spinRate*Time.deltaTime, 0);
	}
}
