using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
