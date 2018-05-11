using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechRecognition : Singleton<SpeechRecognition>
{
    public float silenceTreshold = 10;

    public float detectionTimeout = 60;

    [SerializeField]
    private UnityEngine.UI.Image mPanel;

    [SerializeField]
    private UnityEngine.UI.Text mStatus;

    private DictationRecognizer dictationRecogniser;

    private float lastWordSaid, lastSilenceRaised;

    // Use this for initialization
    void Start()
    {
        dictationRecogniser = new DictationRecognizer();

        dictationRecogniser.InitialSilenceTimeoutSeconds = detectionTimeout;
        dictationRecogniser.AutoSilenceTimeoutSeconds = detectionTimeout;

        lastWordSaid = Time.time;

        dictationRecogniser.DictationResult += (text, confidence) =>
        {
            mPanel.color = Color.green;
            mStatus.text = text;

            Debug.LogFormat("Dictation result: {0}", text);
        };

        dictationRecogniser.DictationHypothesis += (text) =>
        {
            //Debug.LogFormat("Dictation hypothesis: {0}", text);
            lastWordSaid = Time.time;
            //m_Hypotheses.text += text;
        };

        dictationRecogniser.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogWarningFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        };

        dictationRecogniser.DictationError += (error, hresult) =>
        {
            Debug.LogWarningFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

        dictationRecogniser.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (dictationRecogniser != null && 
            dictationRecogniser.Status == SpeechSystemStatus.Running)
        {

            float time = Time.time;
            if (time > lastWordSaid + silenceTreshold)
            {
                if (time > lastSilenceRaised + 5)
                {
                    mPanel.color = Color.red;
                    mStatus.text = string.Format("Too long silence: {0}s.", (int)(time - lastWordSaid));
                    lastSilenceRaised = time;
                }
            }
            else if(mPanel.color == Color.red)
            {
                mPanel.color = Color.green;
                mStatus.text = "Speaking ...";
            }
        }
        else
        {
            mPanel.color = Color.red;
            mStatus.text = "Speech detection stopped";
        }
    }
}