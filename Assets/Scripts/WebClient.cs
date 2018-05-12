using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Text;

public class WebClient : MonoBehaviour {

	#region Delegates

	/// <summary>
	/// Delegate for text recive, extendable with custom objects, at Get method pass this object in call.
	/// </summary>
	public delegate void OnReceive(string text, bool success, params object[] extensions);

	/// <summary>
	/// Delegate for image recive, extendable with custom objects, at GetImage method pass this object in call.
	/// </summary>
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

	/// <summary>
	/// Instantiates a server connection.
	/// </summary>
	public WebClient(MonoBehaviour coroutineHost)
	{
		this.coroutineHost = coroutineHost;
		this.coroutineHost.StartCoroutine(MainThreadExecution());
	}

	/// <summary>
	/// Executes the next web request from the queue if the execution pipeline is free.
	/// </summary>
	/// <returns>Indication for if a new task started executing.</returns>
	public bool ProcessNextRequest()
	{
		while (requestQueue.Count > 0)
		{
			// Grab the next request in the queue
			var request = requestQueue.Dequeue();

			// Process the request
			ProcessRequest(request);

			// Done
			return true;
		}

		return false;
	}

	/// <summary>
	/// Creates a new text request and adds it to the queue.
	/// </summary>
	/// <param name="url"></param>
	/// <param name="onRecive"></param>
	/// <param name="cacheable"></param>
	/// <param name="reliable">If true, the request will get added back to the queue in case of failure.</param>
	/// <param name="service"></param>
	/// <param name="extensions">Additional references to forward to the completion callback.</param>
	public void Get(string url, OnReceive onRecive, params object[] extensions)
	{
		requestQueue.Enqueue(new TextRequest(url, onRecive, extensions));
	}

	/// <summary>
	/// Creates a new image request and adds it to the queue.
	/// </summary>
	/// <param name="url"></param>
	/// <param name="onRecive"></param>
	/// <param name="cacheable"></param>
	/// <param name="reliable">If true, the request will get added back to the queue in case of failure.</param>
	/// <param name="service"></param>
	/// <param name="extensions">Additional references to forward to the completion callback.</param>
	public void GetImage(string url, OnImageReceive onRecive, params object[] extensions)
	{
		requestQueue.Enqueue(new ImageRequest(url, onRecive, extensions));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="request"></param>
	/// <exception cref="NotImplementedException"></exception>
	private void ProcessRequest(Request request)
	{
		request.attempts--;

		if (coroutineHost != null)
			coroutineHost.StartCoroutine(ProcessRequestWithWWW(request));
		else
			Debug.LogError("Failed to process request with WWW. Coroutine host is null.");
		
	}

	#region Helper Methods

	/// <summary>
	/// Processes the spcified request with Unity's WWW service.
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	private IEnumerator ProcessRequestWithWWW(Request request)
	{
		// Wait for other requests to complete
		while (wwwInProgress)
			yield return null;

		wwwInProgress = true;
		var startTime = Time.time;
		var www = new WWW(request.url);

		while (!www.isDone)
		{
			// Wait
			yield return null;
		}
		wwwInProgress = false;

		if (!string.IsNullOrEmpty(www.error))
		{
			// Request failed
			OnRequestFailed(request, www.error);
		}
		else
		{
			if (request is ImageRequest)
			{
				// Process the image
				var resultingTexture = new Texture2D(4, 4);
				if (!resultingTexture.LoadImage(www.bytes))
				{
					OnRequestFailed(request, "The acquired data was not eligible to be used as a texture.");
				}
				else
				{
					// Success
					OnRequestSucceeded(request, resultingTexture);
				}
			}
			else
			{
				// Success
				OnRequestSucceeded(request, www.text);
			}
		}
	}

	/// <summary>
	/// Called when a network request completes.
	/// </summary>
	/// <param name="request"></param>
	/// <param name="result">The resulting data from the request.</param>
	/// <exception cref="InvalidCastException"></exception>
	/// <exception cref="NotImplementedException"></exception>
	private static void OnRequestSucceeded(Request request, object result)
	{
		var textRequest = request as TextRequest;
		if (textRequest != null)
		{
			// Get a reference to the resulting text
			var text = result as string;
			if (text == null) throw new InvalidCastException();

			// Invoke completion callback
			textRequest.onRecive(text, true, request.extensions);
		}
		else
		{
			var imageRequest = request as ImageRequest;
			if (imageRequest != null)
			{
				// Get a reference to the resulting texture
				var texture = result as Texture2D;
				if (texture == null) throw new InvalidCastException();

				// Invoke completion callback
				imageRequest.onRecive(texture, true, string.Empty, imageRequest.extensions);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}

	/// <summary>
	/// Called when a network request fails to complete.
	/// </summary>
	/// <param name="request"></param>
	/// <param name="errorMessage">Brief description of the error.</param>
	/// <exception cref="NotImplementedException"></exception>
	private void OnRequestFailed(Request request, string errorMessage)
	{
		Debug.LogWarning(errorMessage);

		// If the request is not marked for multiple attempts or the number of attempts exceeded the limit
		if (request.attempts < 1)
		{
			if (request.attempts < 1)
				Debug.LogWarning("Request exceeded its number of attempt to perform. Returning null.");

			var textRequest = request as TextRequest;
			if (textRequest != null)
			{
				// Invoke callback with null
				textRequest.onRecive(null, false, textRequest.extensions);
			}
			else
			{
				var imageRequest = request as ImageRequest;
				if (imageRequest != null)
				{
					// Invoke callback with null
					imageRequest.onRecive(null, false, errorMessage, imageRequest.extensions);
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}
		else
		{
			// Put it back to the queue for trying at a later time
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
		/// <summary>
		/// URL of the web request.
		/// </summary>
		public string url;

		/// <summary>
		/// Number of attempts
		/// </summary>
		public int attempts = 3;

		/// <summary>
		/// Additional references to forward to the completion callback.
		/// </summary>
		public object[] extensions;
	}

	private class TextRequest : Request
	{
		public readonly OnReceive onRecive;

		public TextRequest(string url, OnReceive onRecive, object[] extensions)
		{
			this.url = url;
			this.onRecive = onRecive;
			this.extensions = extensions;
		}
	}

	private class ImageRequest : Request
	{
		public readonly OnImageReceive onRecive;

		public ImageRequest(string url, OnImageReceive onRecive, object[] extensions)
		{
			this.url = url;
			this.onRecive = onRecive;
			this.extensions = extensions;
		}
	}

	#endregion
}
