using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI bestScoreText;

    public int score = 0;
    public float finalScore = 0f;

    private const string BEST_SCORE_KEY = "BEST_SCORE";

    [SerializeField] private float scoreCountDuration = 5f;

    private Coroutine scoreRoutine;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        LoadBestScore();
    }

    // ----------------------------
    // SCORE UPDATE DURING GAME
    // ----------------------------
    public void UpdateScore(int pointsToAdd)
    {
        score += pointsToAdd;
        Debug.Log(score);
    }

    // ----------------------------
    // CALLED AT END GAME
    // ----------------------------
    public void UpdateFinalScore()
    {
        if (scoreRoutine != null)
            StopCoroutine(scoreRoutine);

        if (finalScore > 0)
            SoundManager.Instance.PlaySound(Sound.GameWin);

        else 
            SoundManager.Instance.PlaySound(Sound.GameLose);

        scoreRoutine = StartCoroutine(AnimateFinalScore());
    }

    // ----------------------------
    // BEST SCORE LOGIC
    // ----------------------------
    void LoadBestScore()
    {
        int best = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        bestScoreText.text = best + " Points";
    }

    void SaveBestScore(int currentScore)
    {
        int best = PlayerPrefs.GetInt(BEST_SCORE_KEY, -1);

        // If no score exists OR new record
        if (best == -1 || currentScore > best)
        {
            PlayerPrefs.SetInt(BEST_SCORE_KEY, currentScore);
            PlayerPrefs.Save();

            best = currentScore;
        }

        bestScoreText.text = best + " Points";
    }

    IEnumerator AnimateFinalScore()
    {
        yield return new WaitForSeconds(2f);
        float duration = scoreCountDuration;
        float timer = 0f;

        int targetScore = Convert.ToInt32(finalScore);
        int displayedScore = 0;
        SoundManager.Instance.PlayLoop(Sound.PointsCountUp);


        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            // Ease-out curve (fast start, slow end)
            float t = timer / duration;
            t = 1f - Mathf.Pow(1f - t, 3f);

            displayedScore = Mathf.RoundToInt(Mathf.Lerp(0, targetScore, t));

            finalScoreText.text = displayedScore + " Points";

            yield return null;
        }

        // Ensure exact final value
        finalScoreText.text = targetScore + " Points";
        SoundManager.Instance.StopLoop(Sound.PointsCountUp);

        // Save best score AFTER animation finishes
        SaveBestScore(targetScore);
    }

}
