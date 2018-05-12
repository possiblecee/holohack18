using UnityEngine;

/// <summary>
/// Represents a single member in the audience.
/// </summary>
//[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Animator))]
public class AudienceMember : MonoBehaviour
{
    //private Renderer _renderer;
    private bool _isWatched;
    private bool _isPresentationOngoing;
    private float _timeStartedIgnoring;
    private Animator animatorComp;

    /// <summary>
    /// Total time in seconds the player spent ignoring this member during the presentation.
    /// </summary>
    public float TotalTimeBeingIgnored { get; private set; }

    private void Awake()
    {
        // Acquire required components
        //_renderer = GetComponent<Renderer>();
        animatorComp = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayerController.OnPresentationStateChanged += PlayerController_OnPresentationStateChanged;
    }

    private void OnDisable()
    {
        PlayerController.OnPresentationStateChanged -= PlayerController_OnPresentationStateChanged;
    }

    public void SetInterest(float interest) {
        if(this.animatorComp)
            this.animatorComp.SetFloat("Interest", interest);
    }

    /// <summary>
    /// Toggles the watched state of the audience member.
    /// </summary>
    /// <param name="isWatched"></param>
    public void ToggleWatched(bool isWatched)
    {
        if (_isWatched != isWatched)
        {
            //_renderer.material.color = isWatched ? Color.yellow : Color.white;

            if (_isPresentationOngoing)
            {
                if (isWatched)
                {
                    // Player started watching this member
                    TotalTimeBeingIgnored += Time.time - _timeStartedIgnoring;
                }
                else
                {
                    // Player started ignoring this member
                    _timeStartedIgnoring = Time.time;
                }
            }
        }

        // Cached watched state
        _isWatched = isWatched;
    }

    private void PlayerController_OnPresentationStateChanged(object sender, PlayerController.PresentationStateChangedArgs e)
    {
        if (e.isStarted)
        {
            // Start being ignored by the player
            _isWatched = false;
        }
        // If the presentation finishes with this member being ignored
        else if (!_isWatched)
        {
            // Add the last time interval spent by ignoring this member
            TotalTimeBeingIgnored += Time.time - _timeStartedIgnoring;
        }

        // Reset timer
        _timeStartedIgnoring = Time.time;

        // Cache presentation state
        _isPresentationOngoing = e.isStarted;
    }
}
