using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using MadCreep.Enemies;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadCreep {
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyManager : MonoBehaviour {
        public List<PatrolPoint> patrolPoints;
        public float patrolSpeed = 2.5f;
        public float chaseSpeed = 4f;

        [Header("Visibility Settings")]
        public Transform target;
        public float visibilityAngle = 90f;
        public float visibilityDistance = 10f;
        public LayerMask visibilityIgnoreLayers;

        [Header("Audibility Settings")]
        public float hearingDistance = 5f;

        [Header("Combat Settings")]
        public float attackRange = 3f;
        public LayerMask playerLayers;

        public Combat PlayerCombat { get; private set; }
        public EnemyVisibility Visibility { get; private set; }
        public EnemyHearing Hearing { get; private set; }
        public EnemyPatrol EnemyPatrol { get; private set; }
        public bool IsDead { get; private set; } = false;
        
        NavMeshAgent agent;
        State state;

        enum State { None, Patrol, Chase, Dead };

        void Start() {
            agent = GetComponent<NavMeshAgent>();

            EnemyPatrol = new EnemyPatrol(agent, patrolPoints);
            PlayerCombat = new Combat(
                transform: transform,
                adversaryLayers: playerLayers,
                attackRadius: attackRange);
            
            Visibility = new EnemyVisibility(
                transform: transform,
                target: target,
                maxAngle: visibilityAngle,
                maxDistance: visibilityDistance,
                ignoreLayers: visibilityIgnoreLayers);

            Hearing = new EnemyHearing(
                transform: transform,
                target: target,
                maxDistance: hearingDistance
            );

            state = State.None;
        }

        void Update() {
            var oldState = state;

            // Calculate new state
            if (IsDead) {
                state = State.Dead;
            } else if (IsTargetSensed()) {
                state = State.Chase;
            } else {
                state = State.Patrol;
            }

            // Do we need to transition?
            if (state != oldState) {
                
                if (state == State.Patrol) {
                    StartCoroutine(Patrol());
                } else if (state == State.Chase) {
                    StartCoroutine(Chase());
                } else if (state == State.Dead) {
                    StartCoroutine(Dead());
                }
            }
        }

        public void Die() {
            Debug.Log("Die!");
            IsDead = true;
        }

        bool IsTargetSensed() => target != null
               && (Visibility.IsTargetVisible() || Hearing.IsTargetAudible());

        IEnumerator Patrol() {
            agent.speed = patrolSpeed;
            
            while (state == State.Patrol) {
                EnemyPatrol.Patrol();
                yield return null;
            }

            if (agent.enabled) {
                EnemyPatrol.Stop();
            }
        }

        IEnumerator Chase() {
            // Don't start until the next frame
            yield return null;
            
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            
            while (state == State.Chase) {
                agent.SetDestination(target.position);
                PlayerCombat.Attack(collider => {
                    collider.GetComponent<PlayerManager>()?.Die();
                    target = null;
                });

                yield return null;
            }
        }

        IEnumerator Dead() {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            transform.position += Vector3.up * 0.6f;

            agent.enabled = false;
            enabled = false;

            yield return null;
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(EnemyManager))]
    public class EnemyManagerEditor : Editor {
        void OnSceneGUI() {
            var enemy = target as EnemyManager;

            enemy.Visibility?.DrawVisionArc();
        }
    }
    #endif
}
