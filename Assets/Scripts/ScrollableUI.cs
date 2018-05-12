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
    private RawImage rawImage;

    [SerializeField]
    private Image loadingImage;

    [SerializeField]
    private GameObject tapToStartGameObject;

    private SlideLoader slideLoader;

    private void Start()
    {
        rawImage.enabled = false;
        tapToStartGameObject.SetActive(false);

        slideLoader = GetComponent<SlideLoader>();
        slideLoader.LoadTestSlide(GetSlides);

        loadingImage.gameObject.SetActive(true);
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
        rawImage.enabled = true;
        if (SlideCache.Instance.IsReady) rawImage.texture = SlideCache.Instance.Current;
    }

    public void NextObject()
    {
        if (SlideCache.Instance.HasMoreSlides) rawImage.texture = SlideCache.Instance.GetNext();
    }

    public void PrevObject()
    {
        if (SlideCache.Instance.HasPreviousSlide) rawImage.texture = SlideCache.Instance.GetPrevious();
    }
}