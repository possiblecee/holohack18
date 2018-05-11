using UnityEngine;

/// <summary>
/// Represents a single member in the audience.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class AudienceMember : MonoBehaviour
{
    private Renderer _renderer;
    private bool _isWatched;

    private void Awake()
    {
        // Acquire required components
        _renderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// Toggles the watched state of the audience member.
    /// </summary>
    /// <param name="isWatched"></param>
    public void ToggleWatched(bool isWatched)
    {
        if (_isWatched != isWatched)
        {
            _renderer.material.color = isWatched ? Color.yellow : Color.white;
        }
        _isWatched = isWatched;
    }
}
