using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ValueModifierManager {
	public float value {
		get {
			UpdateModifiers();

			float v = 1;
			for (int i = 0; i < modifiers.Count; i++) {
				ValueModifier modifier = modifiers[i];
				v = modifier.Apply(v);
			}
			return v;
		}
	}

	private List<ValueModifier> modifiers = new List<ValueModifier>();

	public void AddModifier(ValueModifier modifier) {
		modifiers.Add(modifier);
	}

	public void RemoveModifier(ValueModifier modifier) {
		modifiers.Remove(modifier);
	}

	public void UpdateModifiers() {
		for (int i = 0; i < modifiers.Count; i++) {
			ValueModifier modifier = modifiers[i];
			if (modifier.remainingTime <= 0) {
				modifiers.RemoveAt(0);
				i--;
			}
		}
	}
}
