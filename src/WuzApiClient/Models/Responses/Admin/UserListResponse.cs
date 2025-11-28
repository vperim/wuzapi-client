namespace WuzApiClient.Models.Responses.Admin;

/// <summary>
/// Response containing list of users.
/// The API returns the user array directly in the data field.
/// </summary>
public sealed class UserListResponse
{
    /// <summary>
    /// Gets or sets the users.
    /// </summary>
    public UserResponse[] Users { get; set; } = [];

    /// <summary>
    /// Creates a UserListResponse from an array of users.
    /// </summary>
    internal static UserListResponse FromArray(UserResponse[] users) => new() { Users = users };
}
