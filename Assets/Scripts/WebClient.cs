using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Text;

public class WebClient : MonoBehaviour {

	#region Delegates

	public delegate void OnReceive(string text, bool success, params object[] extensions);

	public delegate void OnImageReceive(Texture2D texture, bool success, string errorText, params object[] extensions);

	#endregion

	#region Fields

	private MonoBehaviour coroutineHost;
	private readonly Queue<Action> actionQueue = new Queue<Action>();
	private object queueLock = new object();
	private bool wwwInProgress;
	public bool downloadInProgress
	{
		get
		{
			return wwwInProgress;
		}
	}
	private readonly Queue<Request> requestQueue = new Queue<Request>();

	public int NumberOfPendingRequests
	{
		get { return requestQueue.Count; }
	}

	#endregion

	public WebClient(MonoBehaviour coroutineHost)
	{
		this.coroutineHost = coroutineHost;
		this.coroutineHost.StartCoroutine(MainThreadExecution());
	}

	public bool ProcessNextRequest()
	{
		while (requestQueue.Count > 0)
		{
			var request = requestQueue.Dequeue();
			ProcessRequest(request);

			return true;
		}
		return false;
	}

	public void Get(string url, OnReceive onReceive)
	{
		requestQueue.Enqueue(new TextRequest(url, onReceive));
	}

	public void GetImage(string url, OnImageReceive onReceive, params object[] extra)
	{
		requestQueue.Enqueue(new ImageRequest(url, onReceive, extra));
	}

	private void ProcessRequest(Request request)
	{
		request.attempts--;

		if (coroutineHost != null)
			coroutineHost.StartCoroutine(ProcessRequestWithWWW(request));
		else
			Debug.LogError("Failed to process request with WWW. Coroutine host is null.");
		
	}

	#region Helper Methods


	private IEnumerator ProcessRequestWithWWW(Request request)
	{
		while (wwwInProgress)
			yield return null;

		wwwInProgress = true;
		var startTime = Time.time;
		var www = new WWW(request.url);

		while (!www.isDone)
		{
			yield return null;
		}
		wwwInProgress = false;

		if (!string.IsNullOrEmpty(www.error))
		{
			OnRequestFailed(request, www.error);
		}
		else
		{
			if (request is ImageRequest)
			{
				var resultingTexture = new Texture2D(4, 4);
				if (!resultingTexture.LoadImage(www.bytes))
				{
					OnRequestFailed(request, "The acquired data was not eligible to be used as a texture.");
				}
				else
				{
					OnRequestSucceeded(request, resultingTexture);
				}
			}
			else
			{
				OnRequestSucceeded(request, www.text);
			}
		}
	}

	private static void OnRequestSucceeded(Request request, object result)
	{
		var textRequest = request as TextRequest;
		if (textRequest != null)
		{
			var text = result as string;
			if (text == null) throw new InvalidCastException();

			textRequest.onReceive(text, true, request);
		}
		else
		{
			var imageRequest = request as ImageRequest;
			if (imageRequest != null)
			{
				var texture = result as Texture2D;
				if (texture == null) throw new InvalidCastException();

				imageRequest.onReceive(texture, true, string.Empty, imageRequest.extra);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}

	private void OnRequestFailed(Request request, string errorMessage)
	{
		if (request.attempts < 1)
		{
			if (request.attempts < 1)
				Debug.LogWarning("Request exceeded its number of attempt to perform. Returning null.");

			var textRequest = request as TextRequest;
			if (textRequest != null)
			{
				textRequest.onReceive(null, false);
			}
			else
			{
				var imageRequest = request as ImageRequest;
				if (imageRequest != null)
				{
					imageRequest.onReceive(null, false, errorMessage, imageRequest.extra);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}
		else
		{
			requestQueue.Enqueue(request);
		}
	}
		
	private IEnumerator MainThreadExecution()
	{
		while (true)
		{
			lock (queueLock)
			{
				while (actionQueue.Count > 0)
				{
					actionQueue.Dequeue()();
				}
			}

			yield return null;
		}
	}

	#endregion

	#region Nested Types

	private abstract class Request
	{
		public string url;

		public int attempts = 3;
	}

	private class TextRequest : Request
	{
		public readonly OnReceive onReceive;

		public TextRequest(string url, OnReceive onReceive)
		{
			this.url = url;
			this.onReceive = onReceive;
		}
	}

	private class ImageRequest : Request
	{
		public readonly OnImageReceive onReceive;

        public object[] extra;

        public ImageRequest(string url, OnImageReceive onReceive, object[] extra)
		{
			this.url = url;
			this.onReceive = onReceive;
			this.extra = extra;
		}
	}

	#endregion
}
