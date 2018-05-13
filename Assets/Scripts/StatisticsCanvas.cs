using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsCanvas : MonoBehaviour {

    public Text Status;
    public Text Time;
    public Text Speed;
    public Text Message;

    private Interpolator interpolator;

    // Use this for initialization
    private void Start()
    {
        interpolator = this.GetComponent<Interpolator>();
        if (interpolator == null)
        {
            interpolator = this.gameObject.AddComponent<Interpolator>();
        }
    }

    // Update is called once per frame
    void Update () {
        interpolator.SetTargetPosition(CameraCache.Main.transform.position);
        interpolator.SetTargetRotation(CameraCache.Main.transform.rotation);

        var stat = SpeechRecognition.Instance.Statistics;
        Time.text = stat.GetTime() + " sec";
        Speed.text = stat.WordCount + " words, " + stat.GetAvgSpeed() + " word / minute";
        Status.text = SpeechRecognition.Instance.Status.ToString();
        if(stat.Texts.Any())
        {
            Message.text = stat.Texts.Last().Text;
        }
    }
}
