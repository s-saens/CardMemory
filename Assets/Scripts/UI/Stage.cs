// Stage(UI) ' s component

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stage : MonoBehaviour
{
    private void Start() {
        Text txt = this.GetComponent<Text>();
        txt.text = "Stage ";
        txt.text += SceneManager.GetActiveScene().buildIndex.ToString();
    }
}