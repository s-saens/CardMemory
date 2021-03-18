using UnityEngine;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour
{

    public Text newHighText;
    public AudioSource completeSound;
    public AudioSource failSound;

    void Start()
    {
        int originalHighscore = PlayerPrefs.GetInt("HighScore", 0);
        int newScore = Manager.score;
        if( newScore > originalHighscore )
        {
            PlayerPrefs.SetInt("HighScore", newScore);
            newHighText.text = "New Highscore!";
        }
        else
        {
            newHighText.text = "Your Highscore is : " + originalHighscore;
        }
        if(Manager.failed)
        {
            failSound.Play();
        }
        else
        {
            completeSound.Play();
        }
    }
}
