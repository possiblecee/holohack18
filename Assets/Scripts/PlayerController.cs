using System;
using UnityEngine;

/// <summary>
/// Stores attributes about the player.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _fieldOfView = 10.0f;
    private bool _didStartPresenting;
    public float FieldOfView { get { return _fieldOfView; } }

    #region Events

    /// <summary>
    /// Callback for when the player changed the state of the presentation.
    /// </summary>
    public static event EventHandler<PresentationStateChangedArgs> OnPresentationStateChanged;

    public class PresentationStateChangedArgs : EventArgs
    {
        public readonly bool isStarted;

        public PresentationStateChangedArgs(bool isStarted)
        {
            this.isStarted = isStarted;
        }
    }

    private void NotifyPresentationStateChanged(bool isStarted)
    {
        if (OnPresentationStateChanged != null)
        {
            OnPresentationStateChanged(this, new PresentationStateChangedArgs(isStarted));
        }
    }

    #endregion

    private void Start()
    {
        // Start the presentation by default (to be called for UI later instead...)
        StartPresenting();
    }

    /// <summary>
    /// Changes and broadcasts the state change of the presentation to be ongoing.
    /// </summary>
    public void StartPresenting()
    {
        if (_didStartPresenting)
            return;

        _didStartPresenting = true;
        NotifyPresentationStateChanged(isStarted: _didStartPresenting);
    }

    /// <summary>
    /// Changes and broadcasts the state change of the presentation to be finished.
    /// </summary>
    public void StopPresenting()
    {
        if (!_didStartPresenting)
            return;

        _didStartPresenting = false;
        NotifyPresentationStateChanged(isStarted: _didStartPresenting);
    }
}
