using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MadCreep {
    public class Combat {
        public float AttackRadius { get; private set; }
        
        Transform transform;
        LayerMask adversaryLayers;

        public delegate void Die(Collider collider);

        public Combat(
                Transform transform,
                LayerMask adversaryLayers,
                float attackRadius = 3f) {
            this.transform = transform;
            this.adversaryLayers = adversaryLayers;
            AttackRadius = attackRadius;
        }
        
        public void Attack(Die f) {
            var adversary = FindAdversaryInRange();
            if (adversary != null) {
                f(adversary);
            }
        }

        Collider FindAdversaryInRange() {
            var colliders = Physics.OverlapSphere(
                position: transform.position,
                radius: AttackRadius,
                layerMask: adversaryLayers);

            if (colliders.Any()) {
                return colliders.Aggregate(MinDistance);
            } else {
                return null;
            }
        }

        Collider MinDistance(Collider c1, Collider c2) {
            var distance1 = Vector3.Distance(transform.position, c1.transform.position);
            var distance2 = Vector3.Distance(transform.position, c2.transform.position);
            
            return distance1 < distance2 ? c1 : c2;
        }
    }
}
