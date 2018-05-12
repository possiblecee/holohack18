using UnityEngine;
using System;

/// <summary>
/// 
/// </summary>
public class AudienceController : MonoBehaviour
{
    /// <summary>
    /// Reference to the player in the scene.
    /// </summary>
    [SerializeField] private PlayerController _player;
    public static EventHandler<WatchStateEventArgs> onWatchStateChange;
    private bool lastState = false;

    public class WatchStateEventArgs : EventArgs {
        public readonly bool Watching;

        public WatchStateEventArgs(bool watching){
            this.Watching = watching;
        }
    }

    public void NotifyWatchModeChange(bool state){
        if(onWatchStateChange != null)
            onWatchStateChange(this, new WatchStateEventArgs(state));
    }

    void OnEnable()
    {
        ScoreCalculator.onScoreChange += ScoreCalculator_OnScoreChange;
    }

    private void ScoreCalculator_OnScoreChange(object sender, ScoreCalculator.ScoreChangeArgs e)
    {
        foreach(var au in this._audience){
            au.SetInterest(e.Score / 1000.0f);
        }
    }

    void OnDisable()
    {
        ScoreCalculator.onScoreChange -= ScoreCalculator_OnScoreChange;
    }

    /// <summary>
    /// Collection of audience members in the room.
    /// </summary>
    [SerializeField] private AudienceMember[] _audience;

    private void FixedUpdate()
    {
        var fieldOfView = _player.FieldOfView;
        var watchingSomeone = false;
        for (int i = 0; i < _audience.Length; i++)
        {
            var member = _audience[i];
            var directionToMember = (member.transform.position - _player.transform.position).normalized;
            var angle = Vector3.Angle(_player.transform.forward, directionToMember);

            // Notify the audience member whether its being watched or not.
            var watched = angle < fieldOfView || 180.0f - angle < fieldOfView;
            member.ToggleWatched(isWatched: watched);
            if(watched)
                watchingSomeone = true;
        }
        if(lastState != watchingSomeone){
            NotifyWatchModeChange(watchingSomeone);
            lastState = watchingSomeone;
        }
    }

    public void SetInterest(float interest){
        foreach(var watcher in this._audience){
            watcher.SetInterest(interest);
        }
    }
}
