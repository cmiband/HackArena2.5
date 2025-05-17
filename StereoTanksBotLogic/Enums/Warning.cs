namespace StereoTanksBotLogic.Enums;

/// <summary>
/// Represents a warning recieved from game server.
/// </summary>
public enum Warning
{
    /// <summary>
    /// Represents warning that is raised when bot
    /// sends more than one BotResponse per game tick.
    /// </summary>
    PlayerAlreadyMadeActionWarning = 0,

    /// <summary>
    /// Represents warning that is raised when bot
    /// sends response without game state id.
    /// </summary>
    SlowResponseWarning = 1,

    /// <summary>
    /// Represents warning that is raised when bot
    /// sends BotResponse when bot is dead.
    /// </summary>
    ActionIgnoredDueToDeadWarning = 2,

    /// <summary>
    /// Represents warning with custom message.
    /// </summary>
    CustomWarning = 3,
}
