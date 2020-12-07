using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using MadCreep.UI;

namespace MadCreep {
    public class GameManager : MonoBehaviour {
        public PauseScreen pauseScreen;

        [Header("Features")]
        public bool playerAttack = false;
        
        State state;

        public enum State { Running, Paused, PlayerLost, PlayerWon };

        void Start() {
            state = State.Running;
        }

        void Update() {
            if (state == State.Running) {
                if (Input.GetButtonDown("Cancel")) {
                        Pause();
                }
            } else if (state == State.Paused) {
                if (Input.GetButtonDown("Cancel")) {
                        Resume();
                }
            }
        }

        public void Pause() {
            if (state == State.Running) {
                pauseScreen?.gameObject.SetActive(true);
                Time.timeScale = 0f;

                state = State.Paused;
            }
        }
        
        public void Resume() {
            if (state == State.Paused) {
                pauseScreen?.gameObject.SetActive(false);
                Time.timeScale = 1f;

                state = State.Running;
            } else if (state == State.PlayerLost || state == State.PlayerWon) {
                Time.timeScale = 1f;

                // Reload scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public void GameOver() {
            if (pauseScreen != null) {
                pauseScreen.StatusText = "Game Over";
                pauseScreen.ResumeText = "Restart";
                pauseScreen.gameObject.SetActive(true);

                state = State.PlayerLost;
            }
        }

        public void Escaped() {
            if (pauseScreen != null) {
                Time.timeScale = 0f;
                
                pauseScreen.StatusText = "You Escaped!";
                pauseScreen.ResumeText = "Restart";
                pauseScreen.gameObject.SetActive(true);

                state = State.PlayerWon;
            }
        }
        
        public void Quit() {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
