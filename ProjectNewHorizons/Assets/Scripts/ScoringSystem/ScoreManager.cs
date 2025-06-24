using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    float score;
    private float cachedScore;
    public Slider scoreSlider;
    public float maxScore = 1000;
    public float startScore = 500;
    public float decreaseSpeed = 1;
    [Tooltip("Multiplies speed when bar is full")] public float speedMultiplier = 1;
    [Range(-1,1)] public float hueShift = 1;
    [Header("Points given")]
    public float scoreDishComplete = 50;
    public float scoreIngredientCorrect = 10; 
    public float scoreIngredientIncorrect = -30; 
    public float score4InARow = 30;
    public float score5InARow = 50;

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
        scoreSlider.maxValue = maxScore;
        score = startScore;
        scoreSlider.value = score - cachedScore;
    }

    public void IncreaseScore(float ptsToAdd)
    {
        score += ptsToAdd;
        if (scoreSlider != null)
        {
            scoreSlider.maxValue = maxScore;
            scoreSlider.value = score - cachedScore;
        }
        if (score - cachedScore >= maxScore) OnScoreFull();
    }

    void OnScoreFull()
    {
        cachedScore += maxScore * .5f;
        scoreSlider.value = score - cachedScore;
        decreaseSpeed *= speedMultiplier;
        Image fill = scoreSlider.fillRect.GetComponent<Image>();
        Color.RGBToHSV(fill.color, out float h, out float s, out float v);
        fill.color = Color.HSVToRGB((h + hueShift) % 1, s, v);
        print("Score bar full!");
    }

    void OnScoreZero()
    {
        print("You fucking suck");
    }

    private void Update()
    {
        score -= decreaseSpeed * Time.deltaTime; 
        scoreSlider.value = score - cachedScore;

        if (score - cachedScore < 0) OnScoreZero();
    }
}
