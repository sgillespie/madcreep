using UnityEngine;
using UnityEngine.UI;

namespace MadCreep.UI {
    public class PauseScreen : MonoBehaviour {
        public Text statusField;
        public Text resumeField;

        public string StatusText { get; set; } = "Paused";
        public string ResumeText { get; set; } = "Resume";
    
        void OnEnable() {
            statusField.text = StatusText;
            resumeField.text = ResumeText;
        }
    }
}
