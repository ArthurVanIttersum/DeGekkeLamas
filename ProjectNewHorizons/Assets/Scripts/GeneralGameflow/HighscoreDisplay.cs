using TMPro;
using UnityEngine;

public class HighscoreDisplay : MonoBehaviour
{
    public TMP_Text highscoreText;
    static HighscoreDisplay instance;
    private void Awake()
    {
        instance = this;
        int highscore;
        if (PlayerPrefs.HasKey("Highscore"))
        {
            highscore = PlayerPrefs.GetInt("Highscore");
        }
        else
        {
            highscore = 0;
            PlayerPrefs.SetInt("Highscore", 0);
        }
        highscoreText.text = $"Highscore: {PlayerPrefs.GetInt("Highscore")}";
    }

    public static void UpdateHighscoretext()
    {
        instance.highscoreText.text = $"Highscore: {PlayerPrefs.GetInt("Highscore")}";
    }
}
