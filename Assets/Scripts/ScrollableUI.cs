using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScrollableUI : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage_Small;

    [SerializeField]
    private RawImage rawImage_Big;   

    [SerializeField]
    private Image loadingImage;

    [SerializeField]
    private GameObject tapToStartGameObject;

    [SerializeField]
    private GameObject scoreUI;

    private SlideLoader slideLoader;

    private void Start()
    {
        rawImage_Small.enabled = false;
        rawImage_Big.enabled = false;

        tapToStartGameObject.SetActive(false);
        scoreUI.SetActive(false);

        slideLoader = GetComponent<SlideLoader>();

        LoadSlides();

        loadingImage.gameObject.SetActive(true);
    }

    private void LoadSlides()
    {
        if (!String.IsNullOrEmpty(URLSingleton.Instance.url))
        {
            slideLoader.LoadSlideWithShortURL(URLSingleton.Instance.url, GetSlides);            
        }
        else
        {
            slideLoader.LoadTestSlide(GetSlides);
        }
    } 

    private void GetSlides(List<string> slidePath)
    {
        SlideCache.Instance.SetResourceUrls(slidePath);  

        loadingImage.gameObject.SetActive(false);
        tapToStartGameObject.SetActive(true);
    }

    public void GetImage()
    {
        tapToStartGameObject.SetActive(false);

        rawImage_Small.enabled = true;
        rawImage_Big.enabled = true;

        if (SlideCache.Instance.IsReady)
        {
            rawImage_Small.texture = SlideCache.Instance.Current;
            rawImage_Big.texture = rawImage_Small.texture;
        }
    }

    public void NextObject()
    {
        if (SlideCache.Instance.HasMoreSlides)
        {
            rawImage_Small.texture = SlideCache.Instance.GetNext();
            rawImage_Big.texture = rawImage_Small.texture;
        }
        else
        {
            EndPresentation();
        }
    }

    public void PrevObject()
    {
        if (SlideCache.Instance.HasPreviousSlide)
        {
            rawImage_Small.texture = SlideCache.Instance.GetPrevious();
            rawImage_Big.texture = rawImage_Small.texture;
        }
    }

    private void EndPresentation()
    {
        scoreUI.SetActive(true);
    }
}