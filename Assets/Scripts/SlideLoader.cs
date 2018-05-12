using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class SlideLoader : MonoBehaviour {

    private static readonly string TestUrl = "http://magnum-force-chicken.westeurope.cloudapp.azure.com/api/convert/https%3A%2F%2Fwww.sample-videos.com%2Fppt%2FSample-PPT-File-500kb.ppt";

    private static readonly string GetSlideUrl = "http://magnum-force-chicken.westeurope.cloudapp.azure.com/api/get";

    public delegate void OnSlidesLoaded(List<string> slideResourcePaths);

    private bool _init;

    private WebClient _client;

    private int? _remainingSlides = null;

    private List<string> _slideResourcePaths;

    void Start()
    {
        _init = true;
        _client = new WebClient(this);
        _slideResourcePaths = new List<string>();
        LoadTestSlide(null);
    }

    public void LoadTestSlide(OnSlidesLoaded callback)
    {
        _slideResourcePaths.Clear();
        _client.Get(TestUrl, OnReceive);
        StartCoroutine(CheckDownloadState(callback));
    }

    IEnumerator CheckDownloadState(OnSlidesLoaded callback)
    {
        yield return new WaitUntil(() => _remainingSlides == 0);

        if (callback != null)
        {
            callback(_slideResourcePaths);
        }
    }

    void OnReceive(string text, bool success, params object[] extensions)
	{
        if (success)
        {
            var result = ConversionResult.CreateFromJSON(text);
            _remainingSlides = result.files.Count;
            foreach (var slideName in result.files)
            {
                var slideUrl = string.Format("{0}/{1}/{2}", GetSlideUrl, result.hash, slideName);
                _client.GetImage(slideUrl, OnSlideImageReceive, result.hash, slideName);
            }
        }
	}

    void OnSlideImageReceive(Texture2D texture, bool success, string errorText, params object[] extensions)
    {
        if (success)
        {
            _remainingSlides--;
            var prefix = extensions[0] as string;
            var fileName = extensions[1] as string;

            var filePath = Application.streamingAssetsPath + "/" + prefix + "_" + fileName;

            _slideResourcePaths.Add(prefix + "_" + fileName);

            FileStream file = File.Open(filePath, FileMode.Create);
            if (texture != null)
            {
                var bytes = texture.EncodeToPNG();
                file.Write(bytes, 0, bytes.Length);
            }
            else
            {
                File.Delete(filePath);
            }
            file.Close();
        }
    }
	
	void Update () 
	{
		if (!_init) 
		{
			return;
		}

		if (_client.NumberOfPendingRequests > 0) 
		{
			_client.ProcessNextRequest ();
		}
	}
}
