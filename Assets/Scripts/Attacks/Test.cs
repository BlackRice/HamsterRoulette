using UnityEngine;
using System.Collections;

namespace Attacks {
	public class Test:Attack {
		public float damage = 25;
		public override void OnAttack(Hamster other) {
			other.Damage(damage, hamster);
		}
	}
}
