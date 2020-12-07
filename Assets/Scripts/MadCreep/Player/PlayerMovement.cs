using UnityEngine;

namespace MadCreep.Player {
    public class PlayerMovement {
        public float CrouchMultiplier { get; private set; }

        Transform transform;
        CharacterController characterController;
        Transform playerMesh;
        bool isCrouching = false;
        float turnVelocity = 0f;
        float turnSmoothTime = 0.1f;

        public PlayerMovement(
            Transform transform,
            CharacterController characterController,
            Transform playerMesh,
            float crouchMultiplier = 0.6f) {

            this.transform = transform;
            this.characterController = characterController;
            this.playerMesh = playerMesh;
            CrouchMultiplier = crouchMultiplier;
        }

        public void Move(float angle, float speed) {
            Rotate(angle);
            
            var direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            characterController.Move(direction * speed);
        }

        public void Rotate(float angle) {
            float smoothAngle = Mathf.SmoothDampAngle(
                current: transform.eulerAngles.y,
                target: angle,
                currentVelocity: ref turnVelocity,
                smoothTime: turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }

        public void ToggleCrouch(float sizeMultiplier = 0.6f) {
            // Toggle crouch state
            isCrouching = !isCrouching;

            // Scale up or down the player mesh
            var yScale = isCrouching ? CrouchMultiplier : 1f / CrouchMultiplier;

            var currentScale = playerMesh.localScale;
            playerMesh.localScale = new Vector3(currentScale.x, currentScale.y * yScale, currentScale.z);

            // Scale up or down the collider
            characterController.height *= yScale;

            var yAdjustment = isCrouching ? -CrouchMultiplier : CrouchMultiplier;
            characterController.center += Vector3.up * yAdjustment;
        }
    }
}
