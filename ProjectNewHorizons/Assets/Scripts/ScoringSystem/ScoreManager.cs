using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int score;
    public Slider scoreSlider;
    public int maxScore = 1000;
    [Header("Points given")]
    public int scoreDishComplete = 50;
    public int scoreIngredientCorrect = 10; 
    public int scoreIngredientIncorrect = -10; 
    public int score4InARow = 30;
    public int score5InARow = 50;

    public static ScoreManager instance;

    [Header("Debug")]
    public int testPts;
    [NaughtyAttributes.Button]
    void TestScoreAdding()
    {
        IncreaseScore(testPts);
    }
    [NaughtyAttributes.Button]
    void ResetScore()
    {
        if (scoreSlider != null) scoreSlider.value = 0;
        score = 0;
    }


    private void Awake()
    {
        if (instance == null) instance = this;
        if (scoreSlider == null) scoreSlider.maxValue = maxScore;
    }

    public void IncreaseScore(int ptsToAdd)
    {
        score += ptsToAdd;
        if (scoreSlider != null)
        {
            scoreSlider.maxValue = maxScore;
            scoreSlider.value = score;
        }
    }
}
