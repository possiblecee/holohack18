using System;
using UnityEngine;
public class ScoreCalculator : MonoBehaviour {

    public bool isWatching;
    private bool calculationStarted = false;
    public bool isSpeeking;
    public float worldPerMin;
    public float watchStartTime;
    public float timeScale = 1.0f;
    private int score = 1000;
    public int imageCount = 10;
    public int increment = 50;
    public float minPerImage = 2;
    private float estimatedPressentationLength;
    private float lastScore;
    private float lastTime;
    public static EventHandler<ScoreChangeArgs> onScoreChange;

    public class ScoreChangeArgs: EventArgs {
        public readonly int Score;

        public ScoreChangeArgs(int score){
            this.Score = score;
        }
    }

    private void NotifyScoreChange(int score){
        if(onScoreChange != null)
            onScoreChange(this, new ScoreChangeArgs(score));
    }

    public void StartCalculation(int imageCount) {
        calculationStarted = true;
        this.imageCount = imageCount;
        estimatedPressentationLength = imageCount * minPerImage;
    }

    private void OnEnable(){
        AudienceController.onWatchStateChange += AudienceController_OnWatchStateChange;
    }

    private void OnDisable(){
        AudienceController.onWatchStateChange -= AudienceController_OnWatchStateChange;
    }

    private void AudienceController_OnWatchStateChange(object sender, AudienceController.WatchStateEventArgs e)
    {
        isWatching = e.Watching;
        if(e.Watching) {
            watchStartTime = Time.time;
        }
        else {
        }
    }

    private void Update()
    {
        if(calculationStarted){
            if(lastTime > Time.time - 1.0f)
                return;
            this.worldPerMin = SpeechRecognition.Instance.Statistics.WordCount;
            this.isSpeeking = SpeechRecognition.Instance.Status == SpeechRecognition.SpeechStatus.Speeking;
            if(isSpeeking)
                score += increment;
            else
                score -= increment;
            if(isWatching)
                score += increment;
            else
                score -= increment;
            if(score < 500)
                score = 500;
            if(score > 1500)
                score = 1500;
            if(lastScore != score)
                NotifyScoreChange(score);
            lastScore = score;
            lastTime = Time.time;
        }
    }
}