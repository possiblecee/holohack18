using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using System.IO;

/// <summary>
/// Used to keep the currently used textures of the presentation on the heap.
/// </summary>
public sealed class SlideCache : SingletonBehaviour<SlideCache>
{
    private string[] _paths;
    private int _currentIdx;
    private Texture2D _prev, _next;

    /// <summary>
    /// Returns the current slide without stepping the slide pointer.
    /// </summary>
    public Texture2D Current { get; private set; }

    /// <summary>
    /// Indicates whether there cache has a next slide available.
    /// </summary>
    public bool HasMoreSlides
    {
        get { return _paths != null && _paths.Length > _currentIdx + 1; }
    }

    public bool HasPreviousSlide
    {
        get { return _paths != null && _paths.Length > 0 && _currentIdx > 0; }
    }

    /// <summary>
    /// Indicates whether it is safe to call Current, GetNext() or GetPrevious().
    /// </summary>
    public bool IsReady
    {
        get { return _prev != null && Current != null && _next != null; }
    }

    /// <summary>
    /// E.g. SlideCache.Instance.SetResourceUrls(new List<string> { "first.jpg", "second.jpg", "third.jpg", "fourth.jpg" });
    /// </summary>
    /// <param name="resourcePaths">Collection of file names (with extension) to be loaded. The files need to be placed in the StreamingAssets folder.</param>
    public void SetResourceUrls(IEnumerable<string> resourcePaths, Action<Texture2D> onCurrentLoaded = null)
    {
        // Clear previously cached textures
        Clear();

        // Assign array of file names
        _paths = resourcePaths.ToArray();
        if (_paths.Length == 0)
        {
            throw new ArgumentException("The provided collection of file names is empty.");
        }

        // Load current texture
        StartCoroutine(LoadTexture(_paths[_currentIdx], onComplete: (texture) => 
        {
            Current = texture;
            _prev = Current;

            if (onCurrentLoaded != null)
            {
                onCurrentLoaded(Current);
            }
        }));

        if (HasMoreSlides)
        {
            // Load next texture
            StartCoroutine(LoadTexture(_paths[_currentIdx + 1], onComplete: texture => _next = texture));
        }
        else
        {
            _next = Current;
        }
    }

    /// <summary>
    /// Clears the cache and resets the current slide pointer.
    /// </summary>
    public void Clear()
    {
        _paths = null;
        _prev = null;
        Current = null;
        _next = null;
        _currentIdx = 0;
    }

    /// <summary>
    /// Returns the next slide as a Texture2D and steps the current slide pointer forward.
    /// </summary>
    /// <returns></returns>
    public Texture2D GetNext()
    {
        if (!HasMoreSlides)
        {
            Debug.LogError("There are no more slides.");
            return null;
        }

        if (_next == null)
        {
            Debug.LogError("The next slide is not ready yet. Call IsReady to check for the state if caching.");
            return null;
        }

        var result = _next;

        // Step current slide state
        _prev = Current;
        Current = _next;
        _next = null;
        _currentIdx++;

        if (_currentIdx + 1 < _paths.Length)
        {
            // Initiate loading the now next slide
            StartCoroutine(LoadTexture(_paths[_currentIdx + 1], onComplete: texture => _next = texture));
        }
        else
        {
            _next = Current;
        }

        return result;
    }

    /// <summary>
    /// Returns the previous slide as a Texture2D and steps the current slide pointer backwards.
    /// </summary>
    /// <returns></returns>
    public Texture2D GetPrevious()
    {
        if (!HasPreviousSlide)
        {
            Debug.LogError("There is no previous slide.");
            return null;
        }

        if (_prev == null)
        {
            Debug.LogError("The previous slide is not ready yet. Call IsReady to check for the state if caching.");
            return null;
        }

        var result = _prev;

        // Step current slide state
        _next = Current;
        Current = _prev;
        _prev = null;
        _currentIdx--;

        if (_currentIdx - 1 >= 0)
        {
            // Initiate loading the now previous slide
            StartCoroutine(LoadTexture(_paths[_currentIdx - 1], onComplete: texture => _prev = texture));
        }
        else
        {
            _prev = Current;
        }

        return result;
    }

    /// <summary>
    /// Loads the specified texture from the StreamingAssets folder.
    /// </summary>
    /// <param name="filename">File name with extension (eg. image.jpg)</param>
    /// <param name="onComplete">Callback for when the process finishes.</param>
    /// <returns></returns>
    private IEnumerator LoadTexture(string filename, Action<Texture2D> onComplete = null)
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, filename);
        if (!File.Exists(filePath)) throw new FileNotFoundException();

        using (WWW www = new WWW("file://" + filePath))
        {
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                var result = www.texture;
                if (result.width > 8 && result.height > 8)
                {
                    if (onComplete != null)
                    {
                        onComplete(result);
                    }

                    Debug.Log("Loaded: " + filename);
                    yield break;
                }
            }

            Debug.LogError("Could not load texture from path " + filename + ". Reason: " + www.error);
        }
    }
}
