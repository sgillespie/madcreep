using UnityEngine;
using UnityEngine.SceneManagement;

namespace MadCreep {
    public class UIManager : MonoBehaviour {
        public const string STARTING_SCENE = "MechanicsDemo";
        
        public void OnClickStart() => SceneManager.LoadScene(STARTING_SCENE);

        public void OnClickQuit() {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
