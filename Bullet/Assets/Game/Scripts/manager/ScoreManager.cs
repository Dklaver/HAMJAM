using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void UpdateScore(int pointsToAdd)
    {
        score = score + pointsToAdd;
        Debug.Log(score);
    }
}
