using UnityEngine;

namespace MadCreep {
    public class PatrolPoint : MonoBehaviour {
        public const float SPHERE_SIZE = 0.25f;
        public const float SPHERE_OFFSET = 1f;
    
        void OnDrawGizmos() {
            var pos = Vector3.up * SPHERE_OFFSET + transform.position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(pos, SPHERE_SIZE);
        }
    }
}
