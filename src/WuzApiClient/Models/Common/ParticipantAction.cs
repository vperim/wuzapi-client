namespace WuzApiClient.Models.Common;

/// <summary>
/// Defines actions for group participant management.
/// </summary>
public enum ParticipantAction
{
    /// <summary>
    /// Add participants to the group.
    /// </summary>
    Add,

    /// <summary>
    /// Remove participants from the group.
    /// </summary>
    Remove,

    /// <summary>
    /// Promote participants to admin.
    /// </summary>
    Promote,

    /// <summary>
    /// Demote participants from admin.
    /// </summary>
    Demote
}
