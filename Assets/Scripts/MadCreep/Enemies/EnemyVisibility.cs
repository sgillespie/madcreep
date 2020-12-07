using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadCreep.Enemies {
    public class EnemyVisibility {
        public Transform Transform { get; private set; }
        public Transform Target { get; private set; }
        public float MaxAngle { get; private set; }
        public float MaxDistance { get; private set; }
        public LayerMask IgnoreLayers { get; private set; }

        public EnemyVisibility(
                Transform transform,
                Transform target,
                float maxAngle,
                float maxDistance,
                LayerMask ignoreLayers) {

            Transform = transform;
            Target = target;
            MaxAngle = maxAngle;
            MaxDistance = maxDistance;
            IgnoreLayers = ignoreLayers;
        }
       
        public bool IsTargetVisible() {
            var direction = Target.position - Transform.position;
            var angle = Vector3.Angle(Transform.forward, direction);

            if (angle > MaxAngle / 2) {
                return false;
            }

            if (direction.magnitude > MaxDistance) {
                return false;
            }

            RaycastHit raycastHit;
            bool raycastResult = Physics.Raycast(
                origin: Transform.position,
                direction: direction,
                layerMask: ~IgnoreLayers,
                hitInfo: out raycastHit,
                maxDistance: MaxDistance);

            return raycastResult && raycastHit.transform == Target;
        }

        #if UNITY_EDITOR
        public void DrawVisionArc() {
            var arcStart = Quaternion.Euler(0f, MaxAngle / -2, 0f)
                * Transform.forward
                * MaxDistance;

            Handles.color = IsTargetVisible() ? new Color(1f, 0f, 0f, 0.3f) : new Color(0f, 1f, 0f, 0.1f);
            Handles.DrawSolidArc(
                center: Transform.position,
                normal: Vector3.up,
                from: arcStart,
                angle: MaxAngle,
                radius: MaxDistance);
        }
        #endif
    }
}
