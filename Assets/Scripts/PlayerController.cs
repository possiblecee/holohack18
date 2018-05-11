using UnityEngine;

/// <summary>
/// Stores attributes about the player.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _fieldOfView = 30.0f;
    public float FieldOfView { get { return _fieldOfView; } }
}
