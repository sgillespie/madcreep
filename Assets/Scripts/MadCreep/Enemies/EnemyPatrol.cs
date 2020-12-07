using System.Collections.Generic;
using UnityEngine;

namespace MadCreep.Enemies {
    public class EnemyPatrol {
        public const float STOPPING_DISTANCE = 0.5f;
        public const bool AUTOBRAKE = false;
        
        public UnityEngine.AI.NavMeshAgent NavMeshAgent { get; private set; }
        public List<PatrolPoint> PatrolPoints { get; private set; }

        int nextPoint = 0;

        public EnemyPatrol(
            UnityEngine.AI.NavMeshAgent navMeshAgent,
            List<PatrolPoint> patrolPoints) {

            NavMeshAgent = navMeshAgent;
            PatrolPoints = patrolPoints;

            NavMeshAgent.autoBraking = AUTOBRAKE;
            NavMeshAgent.stoppingDistance = STOPPING_DISTANCE;
            NavMeshAgent.SetDestination(patrolPoints[0].transform.position);
        }

        public void Patrol() {
            if (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance) {
                nextPoint = (nextPoint + 1) % PatrolPoints.Count;

                NavMeshAgent.SetDestination(PatrolPoints[nextPoint].transform.position);
            }
        }

        public void Stop() {
            NavMeshAgent.isStopped = true;
        }
    }
}
