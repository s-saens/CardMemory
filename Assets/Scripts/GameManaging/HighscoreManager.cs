using UnityEngine;
using UnityEngine.UI;

public class HighscoreManager : MonoBehaviour
{
    private string highscoreKey = "HighScore";

    public bool UpdateHighscore(int newScore)
    {
        int savedScore = PlayerPrefs.GetInt(highscoreKey, 0);
        bool highscoreChanged = false;
        if(newScore > savedScore) {
            PlayerPrefs.SetInt(highscoreKey, newScore);
            highscoreChanged = true;
        }
        return highscoreChanged;
    }

    
}