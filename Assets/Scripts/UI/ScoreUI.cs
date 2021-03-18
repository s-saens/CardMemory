
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    private void Update()
    {
        this.GetComponent<Text>().text = "Score : " + Manager.score;
    }
}
