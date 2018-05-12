using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class SlideLoader : MonoBehaviour {

	private static string TestUrl = "http://magnum-force-chicken.westeurope.cloudapp.azure.com/api/convert/https%3A%2F%2Fwww.sample-videos.com%2Fppt%2FSample-PPT-File-500kb.ppt";

	private bool _init;

	private WebClient client;

	void Start () 
	{
		_init = true;
		client = new WebClient (this);
		client.Get (TestUrl, OnReceive);
	}

	void OnReceive(string text, bool success, params object[] extensions)
	{
		Debug.Log(text);
	}
	
	void Update () 
	{
		if (!_init) 
		{
			return;
		}

		if (client.NumberOfPendingRequests > 0) 
		{
			client.ProcessNextRequest ();
		}
	}

}
