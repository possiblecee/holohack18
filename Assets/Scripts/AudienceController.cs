using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AudienceController : MonoBehaviour
{
    /// <summary>
    /// Reference to the player in the scene.
    /// </summary>
    [SerializeField] private PlayerController _player;

    /// <summary>
    /// Collection of audience members in the room.
    /// </summary>
    [SerializeField] private AudienceMember[] _audience;

    private void FixedUpdate()
    {
        var fieldOfView = _player.FieldOfView;
        for (int i = 0; i < _audience.Length; i++)
        {
            var member = _audience[i];
            var directionToMember = (member.transform.position - _player.transform.position).normalized;
            var angle = Vector3.Angle(_player.transform.forward, directionToMember);

            // Notify the audience member whether its being watched or not.
            member.ToggleWatched(isWatched: angle < fieldOfView);
        }
    }
}
