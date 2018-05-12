using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour {
    private Text scoreText;

    void OnEnable()
    {
        this.scoreText = GetComponent<Text>();
        ScoreCalculator.onScoreChange += ScoreCalculator_OnScoreChange;
    }

    void OnDisable()
    {
        ScoreCalculator.onScoreChange -= ScoreCalculator_OnScoreChange;
    }

    private void ScoreCalculator_OnScoreChange(object sender, ScoreCalculator.ScoreChangeArgs e)
    {
        scoreText.text = "Score: " + e.Score;
    }
}