using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using MadCreep.Enemies;
using MadCreep.Player;

namespace MadCreep {
    [RequireComponent(typeof(CharacterController))]
    public class PlayerManager : MonoBehaviour {
        public new Transform camera;
        public Transform playerMesh;
        public GameManager gameManager;

        [Header("Movement Settings")]
        public float speed = 5f;
        public float crouchMultiplier = 0.6f;

        [Header("Attack Settings")]
        public float attackRange = 1f;
        public LayerMask enemyLayers;

        [Header("Destination")]
        public LayerMask destinationLayers;

        CharacterController player;
        State state = State.None;

        public PlayerMovement Movement { get; private set; }
        public Combat PlayerCombat { get; private set; }
        public bool IsDead { get; private set; } = false;
        float Horizontal => Input.GetAxisRaw("Horizontal");
        float Vertical => Input.GetAxisRaw("Vertical");
        bool Crouch => Input.GetKeyDown(KeyCode.LeftControl);
        bool Attack => Input.GetButtonDown("Fire1");

        enum State { None, Normal, Dead, Escaped };

        void Start() {
            player = GetComponent<CharacterController>();

            Movement = new PlayerMovement(
                transform: transform,
                characterController: player,
                playerMesh: playerMesh,
                crouchMultiplier: crouchMultiplier);

            PlayerCombat = new Combat(
                transform: transform,
                adversaryLayers: enemyLayers,
                attackRadius: attackRange);

            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update() {
            var oldState = state;

            // Calculate new state
            if (IsDead) {
                state = State.Dead;
            } else if (isDestinationReached()) {
                state = State.Escaped;
            } else {
                state = State.Normal;
            }

            // Need to transition?
            if (state != oldState) {
                switch (state) {
                    case State.Normal:
                        StartCoroutine(Normal());
                        break;
                    case State.Dead:
                        StartCoroutine(Dead());
                        break;
                    case State.Escaped:
                        StartCoroutine(Escaped());
                        break;
                }
            }
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public void Die() {
            IsDead = true;
        }

        IEnumerator Normal() {
            while (state == State.Normal) {
                var direction = new Vector3(Horizontal, 0f, Vertical).normalized;
                if (direction.magnitude >= 0.1f) {
                    var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;

                    Movement.Move(targetAngle, speed * Time.deltaTime);
                }

                if (Crouch) {
                    Movement.ToggleCrouch(crouchMultiplier);
                }

                if (gameManager.playerAttack && Attack) {
                    PlayerCombat.Attack(collider =>
                        collider.GetComponent<EnemyManager>()?.Die());
                }

                yield return null;
            }
        }

        IEnumerator Dead() {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            transform.position += Vector3.down * 0.5f;

            gameManager.GameOver();
            player.enabled = false;
            enabled = false;

            yield return null;
        }

        IEnumerator Escaped() {
            yield return new WaitForSeconds(0.5f);

            gameManager.Escaped();
        }

        bool isDestinationReached() {
            return Physics.Raycast(
                origin: playerMesh.position,
                direction: Vector3.down,
                maxDistance: 5f,
                layerMask: destinationLayers);
        }
    }
}
