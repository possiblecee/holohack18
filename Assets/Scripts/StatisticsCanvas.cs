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

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.position = CameraCache.Main.transform.position;
        this.transform.rotation = CameraCache.Main.transform.rotation;

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
