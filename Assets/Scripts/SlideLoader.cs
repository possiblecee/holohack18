using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SlideLoader : MonoBehaviour {

	private static string TestUrl = "http://magnum-force-chicken.westeurope.cloudapp.azure.com/api/convert/https%3A%2F%2Fwww.sample-videos.com%2Fppt%2FSample-PPT-File-500kb.ppt";

    private static string GetSlideUrl = "http://magnum-force-chicken.westeurope.cloudapp.azure.com/api/get";

	private bool _init;

	private WebClient client;

    [System.Serializable]
    class ConversionResult
    {
        public string hash;
        public List<string> files;

        public static ConversionResult CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ConversionResult>(jsonString);
        }

    }

    void Start () 
	{
		_init = true;
		client = new WebClient (this);
		client.Get (TestUrl, OnReceive);
	}

	void OnReceive(string text, bool success, params object[] extensions)
	{
        if (success)
        {
            var result = ConversionResult.CreateFromJSON(text);
            foreach (var slideName in result.files)
            {
                var slideUrl = string.Format("{0}/{1}/{2}", GetSlideUrl, result.hash, slideName);
                client.GetImage(slideUrl, OnSlideImageReceive, result.hash, slideName);
            }
        }
	}

    private string RemoveFileName(string pathString)
    {
        return Path.GetDirectoryName(pathString);
    }

    void OnSlideImageReceive(Texture2D texture, bool success, string errorText, params object[] extensions)
    {
        if (success)
        {
            var folder = extensions[0] as string;
            var fileName = extensions[1] as string;

            var folderPath = Application.streamingAssetsPath + "/" + folder;
            var filePath = folderPath + "_" + fileName;

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

		if (client.NumberOfPendingRequests > 0) 
		{
			client.ProcessNextRequest ();
		}
	}

}
