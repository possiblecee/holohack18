using System.Collections.Generic;
using UnityEngine;

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
