
using UnityEngine;
using UnityEngine.UI;

public class HighscoreUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Text>().text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }
}
