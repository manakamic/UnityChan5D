using UnityEngine;
using UnityEngine.SceneManagement;
using Common;

namespace Title {

    public class Title : MonoBehaviour {

        [SerializeField]
        private uGUI.UIClick ClickEvent;

        private void Start() {
            if (ClickEvent != null) {
                ClickEvent.OnClickEvent = (eventData) => {
                    SceneManager.LoadScene(Defines.SceneNameActionStart);
                };
            }
        }
    }
}