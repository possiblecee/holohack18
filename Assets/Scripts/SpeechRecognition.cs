using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows.Speech;

public class SpeechRecognition : Singleton<SpeechRecognition>
{
    public class SpeechStatistics
    {
        public float StartTime;

        public int BreaksCount = 0;

        public int WordCount = 0;

        public List<SpeechText> Texts = new List<SpeechText>();

        public float GetTime()
        {
            return (int)(Time.time - StartTime);
        }

        public int CalculateWordCount()
        {
            return Texts.Select(t => t.Text.Split(' ').Length).Sum();
        }

        public void AddText(string text)
        {
            Texts.Add(new SpeechText { Text = text, Time = Time.time });
            WordCount += text.Split(' ').Length;
        }

        public float GetAvgSpeed()
        {
            return WordCount / (GetTime() / 60f);
        }

        public float GetActualSpeed()
        {
            return WordCount / (GetTime() / 60f);
        }
    }

    public struct SpeechText
    {
        public string Text;

        public float Time;
    }

    [Serializable]
    public enum SpeechStatus { Silence, Speeking, NotStarted, Stopped }

    public float silenceTreshold = 10;

    public float detectionTimeout = 600;

    public SpeechStatistics Statistics = new SpeechStatistics();

    public SpeechStatus Status = SpeechStatus.NotStarted;

    public UnityEvent StatusChanged;

    public UnityEvent ApplicationReset;

    public bool AutoStart;

    private DictationRecognizer dictationRecogniser;

    private float lastWordSaid;

    private const string RestartCommand = "restart";

    public void StartSpeech()
    {
        dictationRecogniser.Start();
        Statistics = new SpeechStatistics { StartTime = Time.time };
        Status = SpeechStatus.Speeking;

        lastWordSaid = Time.time;
    }

    public void StopSpeech()
    {
        dictationRecogniser.Stop();
    }

    // Use this for initialization
    void Start()
    {
        dictationRecogniser = new DictationRecognizer();

        dictationRecogniser.InitialSilenceTimeoutSeconds = detectionTimeout;
        dictationRecogniser.AutoSilenceTimeoutSeconds = detectionTimeout;

        dictationRecogniser.DictationResult += DictationRecogniser_DictationResult;
        dictationRecogniser.DictationHypothesis += DictationRecogniser_DictationHypothesis;
        dictationRecogniser.DictationComplete += DictationRecogniser_DictationComplete;
        dictationRecogniser.DictationError += DictationRecogniser_DictationError;

        if(AutoStart)
        {
            StartSpeech();
        }
    }

    private void DictationRecogniser_DictationComplete(DictationCompletionCause completionCause)
    {
        if (completionCause != DictationCompletionCause.Complete)
            Debug.LogWarningFormat("Dictation completed unsuccessfully: {0}.", completionCause);

        Status = SpeechStatus.Stopped;
    }

    private void DictationRecogniser_DictationResult(string text, ConfidenceLevel confidence)
    {
        this.Statistics.AddText(text);
        Debug.LogFormat("Dictation result: {0}", text);

        if (text == RestartCommand)
        {
            this.ApplicationReset.Invoke();
        }
    }

    private void DictationRecogniser_DictationHypothesis(string text)
    {
        this.Status = SpeechStatus.Speeking;
        lastWordSaid = Time.time;
    }

    private void DictationRecogniser_DictationError(string error, int hresult)
    {
        Debug.LogWarningFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        Status = SpeechStatus.Stopped;
    }

    // Update is called once per frame
    void Update()
    {
        if (dictationRecogniser != null &&
            this.Status == SpeechStatus.Speeking &&
            dictationRecogniser.Status == SpeechSystemStatus.Running)
        {
            float time = Time.time;
            if (time > lastWordSaid + silenceTreshold)
            {
                if (this.Status != SpeechStatus.Silence)
                {
                    this.Status = SpeechStatus.Silence;
                    this.Statistics.BreaksCount++;
                }
            }
        }

        if (dictationRecogniser != null &&
            this.Status == SpeechStatus.Speeking &&
            dictationRecogniser.Status == SpeechSystemStatus.Stopped)
        {
            Status = SpeechStatus.Stopped;
        }
    }

    private void OnApplicationQuit()
    {
        if (dictationRecogniser != null)
        {
            dictationRecogniser.Stop();
            dictationRecogniser.DictationResult -= DictationRecogniser_DictationResult;
            dictationRecogniser.DictationHypothesis -= DictationRecogniser_DictationHypothesis;
            dictationRecogniser.DictationComplete -= DictationRecogniser_DictationComplete;
            dictationRecogniser.DictationError -= DictationRecogniser_DictationError;
            dictationRecogniser.Dispose();
        }
    }
}