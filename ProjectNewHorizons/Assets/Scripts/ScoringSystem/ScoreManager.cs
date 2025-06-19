using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int score;
    public Slider scoreSlider;
    public int maxScore = 10000;

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
        if (scoreSlider != null) scoreSlider.value = score;
    }
}
