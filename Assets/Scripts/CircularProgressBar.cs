using UnityEngine;
using System.Collections;

public class CircularProgressBar : MonoBehaviour {
	public float startAngle = 30;
	public float endAngle = -30;
	
	private float rangeLength {
		get {
			return Mathf.Abs(startAngle-endAngle);
		}
	}
	public float value {
		get {
			return -((transform.localEulerAngles.z-startAngle)/rangeLength);
		}
		set {
			Vector3 angles = transform.localEulerAngles;
			angles.z = -(value*rangeLength-startAngle);
			transform.localEulerAngles = angles;
		}
	}
}
