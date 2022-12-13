using System.Collections.Generic;

namespace ChessAvalonia.Models
{
    internal static class Errors
    {
        internal enum ErrorReason
        {
            Default,
            LobbyPageCannotConnectToServer,
            LobbyPageConnectionToServerLost,
            MainPageConnectionToServerLost
        }

        internal static Dictionary<ErrorReason, string> ErrorMessages { get; } = new()
        {
            { ErrorReason.Default, "Something went wrong..." },
            { ErrorReason.LobbyPageCannotConnectToServer, "Cannot connect to the server..." },
            { ErrorReason.LobbyPageConnectionToServerLost, "Lost connection to the server..." },
            { ErrorReason.MainPageConnectionToServerLost, "Lost connection to the server..." }
        };
    }
}
