using UnityEngine;
using UnityEngine.SceneManagement;
using Common;

public class Title : MonoBehaviour {
    public void OnPushStart() {
        SceneManager.LoadScene(Defines.SceneNameActionStart);
    }
}
