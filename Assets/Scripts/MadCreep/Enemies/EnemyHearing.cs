using UnityEngine;

namespace MadCreep.Enemies {
    public class EnemyHearing {
        public Transform Transform { get; private set; }
        public Transform Target { get; private set; }
        public float MaxDistance { get; private set; }

        public EnemyHearing(
                Transform transform,
                Transform target,
                float maxDistance) {

            Transform = transform;
            Target = target;
            MaxDistance = maxDistance;
        }

        public bool IsTargetAudible() {
            var direction = Target.position - Transform.position;

            return direction.magnitude < MaxDistance;
        }
    }
}
