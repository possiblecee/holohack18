using UnityEngine;

public class EditorOnly : MonoBehaviour{
    #if !UNITY_EDITOR
    void Start()
    {
        this.gameObject.SetActive(false);
    }
    #endif
}