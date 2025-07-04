using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    float score;
    float trueScore;
    private float cachedScore;
    public Slider scoreSlider;
    public TMP_Text scoreDisplay;
    public float maxScore = 1000;
    public float startScore = 500;
    public float decreaseSpeed = 1;
    public float updateSpeed = 1;
    public UnityEvent onGameLost;
    [Header("Boost stuff")]
    public float boostDuration = 3;
    public float boostMultiplier = 2;
    public Color boostColor;
    private float currentMultiplier = 1;
    private bool boostActive;
    [Tooltip("Multiplies speed when bar is full")] public float speedMultiplier = 1;
    [Range(-1,1)] public float hueShift = 1;
    [Header("Points given")]
    public float scoreDishComplete = 50;
    public float scoreIngredientCorrect = 10; 
    public float scoreIngredientIncorrect = -30; 

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
        trueScore = 0;
    }


    private void Awake()
    {
        if (instance == null) instance = this;
        scoreSlider.maxValue = maxScore;
        score = startScore;
        scoreSlider.value = score - cachedScore;
        if (scoreDisplay != null) scoreDisplay.text = $"Score: {Mathf.RoundToInt(trueScore)}";
    }

    public void IncreaseScore(float ptsToAdd)
    {
        if (!boostActive) score += ptsToAdd * currentMultiplier;
        trueScore += ptsToAdd * currentMultiplier;
        if (scoreSlider != null)
        {
            scoreSlider.maxValue = maxScore;
            //scoreSlider.value = score - cachedScore;
        }
        if (score - cachedScore >= maxScore && !boostActive) OnScoreFull();
        if (scoreDisplay != null) scoreDisplay.text = $"Score: { Mathf.RoundToInt(trueScore)}";

        if (trueScore > PlayerPrefs.GetInt("Highscore"))
        {
            PlayerPrefs.SetInt("Highscore", (int)trueScore);
            HighscoreDisplay.UpdateHighscoretext();
        }
    }

    void OnScoreFull()
    {
        //scoreSlider.value = score - cachedScore;
        decreaseSpeed *= speedMultiplier;
        Image fill = scoreSlider.fillRect.GetComponent<Image>();
        Color.RGBToHSV(fill.color, out float h, out float s, out float v);
        fill.color = Color.HSVToRGB((h + hueShift) % 1, s, v);
        print("Score bar full!");
        StartCoroutine(ScoreBoost());
    }

    IEnumerator ScoreBoost()
    {
        scoreSlider.value = maxScore;
        Image fill = scoreSlider.fillRect.GetComponent<Image>();
        Color cachedColor = fill.color;
        fill.color = boostColor;
        boostActive = true;
        currentMultiplier = boostMultiplier;

        yield return new WaitForSeconds(boostDuration);

        currentMultiplier = 1;
        boostActive = false;
        fill.color = cachedColor;
        cachedScore += (score - cachedScore) * .5f;
        yield return new();
    }

    void OnScoreZero()
    {
        print("You fucking suck");
        onGameLost.Invoke();
        MatchingDetection.isPaused = true;
    }

    private void Update()
    {
        if (!boostActive)
        {
            if (GridActivator.isPlayingMatch3)
            {
                score -= decreaseSpeed * Time.deltaTime;
                if (score - cachedScore < 0) OnScoreZero();
            }
            scoreSlider.value += (score - cachedScore - scoreSlider.value) * Time.deltaTime * updateSpeed;
        }
    }
}
