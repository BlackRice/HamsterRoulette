using UnityEngine;
using System.Collections;

public class ValueModifier {
	public enum Type
	{
		Multiply,
		Min,
		Max
	}

	public float value { get; set;}
	public float lifespan { get; set;}
	public Type type { get; set;}

	public float elapsedTime {
		get {
			return Time.timeSinceLevelLoad-startTime;
		}
	}

	public float remainingTime {
		get {
			return lifespan-Mathf.Clamp(elapsedTime, 0, lifespan);
		}
	}


	private float startTime;

	public ValueModifier(float value, float lifespan, Type type) {
		startTime = Time.timeSinceLevelLoad;
		this.value = value;
		this.lifespan = lifespan;
		this.type = type;
	}

	public float Apply(float inputValue) {
		if (type == Type.Multiply) {
			inputValue *= value;
		}
		else if (type == Type.Min) {
			inputValue = Mathf.Min(inputValue, value);
		}
		else if (type == Type.Max) {
			inputValue = Mathf.Max(inputValue, value);
		}
		return inputValue;
	}
}
