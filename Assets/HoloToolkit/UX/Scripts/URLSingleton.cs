using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLSingleton : MonoBehaviour
{
    private static URLSingleton _instance;

    public string url { get; set; }

    public static URLSingleton Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new URLSingleton();
            }

            return _instance;
        }
    }

    // Use this for initialization
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}