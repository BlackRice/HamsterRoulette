using UnityEngine;
using System.Collections;

namespace Attacks {
	public class MeleeAttack:Attack {
		public float hitRadius = 0.25f;
		public float moveSpeed = 0.3f;

		public AnimationCurve attackCurve = new AnimationCurve();

		protected override IEnumerator OnAttack(Hamster other) {
			float totalTime = moveSpeed*Vector3.Distance(transform.position, other.transform.position);
			float endTime = Time.timeSinceLevelLoad+totalTime*2;
			float startTime = Time.timeSinceLevelLoad;
			
			bool wasHalfDone = false;
			Vector3 hitPosition = Vector3.zero;
			
			while (Time.timeSinceLevelLoad < endTime) {
				if (!hamster || !other)
				{
					break;
				}

				float factor = (Time.timeSinceLevelLoad-startTime)/totalTime;
				bool isHalfDone = factor >= 1.0f;
				
				if (isHalfDone) {
					factor = 1.0f-(factor-1.0f);
					if (!wasHalfDone) {
						other.Damage(GetDamage(other), hamster);
						hitPosition = hamster.transform.localPosition;
						wasHalfDone = true;
					}
				}

				factor = Mathf.Clamp01(factor);
				factor = attackCurve.Evaluate(factor);

				Vector3 startPosition = hamster.idleLocalPosition;
				Vector3 endPosition = Vector3.zero;
				
				if (isHalfDone) {
					endPosition = hitPosition;
				}
				else {
					Vector3 dir = (hamster.transform.position-other.transform.position).normalized;
					Vector3 pos = other.transform.position+dir*hitRadius;
					endPosition = hamster.transform.parent.InverseTransformPoint(pos);
				}
				
				Vector3 localPosition = Vector3.Lerp(startPosition, endPosition, factor);
				
				hamster.transform.localPosition = localPosition;
				
				yield return 0;
			}
		}
	}
}
