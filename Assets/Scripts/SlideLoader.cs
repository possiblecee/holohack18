﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Web;

public class SlideLoader : MonoBehaviour
{

    private static readonly string BaseUrl = "http://magnum-force-chicken.westeurope.cloudapp.azure.com/api";

    public delegate void OnSlidesLoaded(List<string> slideResourcePaths);

    private bool _init;

    private WebClient _client;

    private int? _remainingSlides = null;

    private List<string> _slideResourcePaths;

    void Awake()
    {
        _init = true;
        _client = new WebClient(this);
        _slideResourcePaths = new List<string>();
    }

    public void LoadSlide(string slideUrl, OnSlidesLoaded callback)
    {
        _slideResourcePaths.Clear();
        var url = string.Format("{0}/{1}/{2}", BaseUrl, "convert", WWW.EscapeURL(slideUrl));
        _client.Get(url, OnReceive);
        StartCoroutine(CheckDownloadState(callback));
    }

    public void LoadSlideWithShortURL(string shortURL, OnSlidesLoaded callback)
    {
        _slideResourcePaths.Clear();
        _client.Get(shortURL, OnReceive);
        StartCoroutine(CheckDownloadState(callback));
    }

    public void LoadTestSlide(OnSlidesLoaded callback)
    {
        var testUrl = "http://magnum-force-chicken.westeurope.cloudapp.azure.com/api/convert/https%3A%2F%2Fwww.sample-videos.com%2Fppt%2FSample-PPT-File-500kb.ppt";
        _slideResourcePaths.Clear();
        _client.Get(testUrl, OnReceive);
        StartCoroutine(CheckDownloadState(callback));
    }

    IEnumerator CheckDownloadState(OnSlidesLoaded callback)
    {
        yield return new WaitUntil(() => _remainingSlides == 0);

        if (callback != null)
        {
            callback(_slideResourcePaths);
        }
        _remainingSlides = null;
    }

    void OnReceive(string text, bool success, params object[] extensions)
    {
        if (success)
        {
            var result = ConversionResult.CreateFromJSON(text);
            _remainingSlides = result.files.Count;
            foreach (var slideName in result.files)
            {
                var slideUrl = string.Format("{0}/{1}/{2}/{3}", BaseUrl, "get", result.hash, slideName);
                _client.GetImage(slideUrl, OnSlideImageReceive, result.hash, slideName);
            }
        }
        else
        {
            Debug.LogError("Conversion failed.");
            _remainingSlides = 0;
        }
    }

    void OnSlideImageReceive(Texture2D texture, bool success, string errorText, params object[] extra)
    {
        if (success)
        {
            _remainingSlides--;
            var prefix = extra[0] as string;
            var fileName = extra[1] as string;

            Directory.CreateDirectory(Application.streamingAssetsPath);

            var filePath = Application.streamingAssetsPath + "/" + prefix + "_" + fileName;

            _slideResourcePaths.Add(prefix + "_" + fileName);

            using (var file = File.Open(filePath, FileMode.Create))
            {
                if (texture != null)
                {
                    var bytes = texture.EncodeToPNG();
                    file.Write(bytes, 0, bytes.Length);
                }
                else
                {
                    File.Delete(filePath);
                }
            }
        }
    }

    void Update()
    {
        if (!_init)
        {
            return;
        }

        if (_client.NumberOfPendingRequests > 0)
        {
            _client.ProcessNextRequest();
        }
    }
}