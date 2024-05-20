namespace PhigrosLibraryCSharp.Cloud.DataStructure;

/// <summary>
/// The user's info in game.
/// </summary>
/// <param name="ShowUserId"><see langword="true"/> if user has the user name expanded, otherwise <see langword="false"/>.</param>
/// <param name="Intro">User's intro.</param>
/// <param name="AvatarId">User's avatar id.</param>
/// <param name="BackgroundId">User's background id.</param>
public record class GameUserInfo(bool ShowUserId, string Intro, string AvatarId, string BackgroundId);
