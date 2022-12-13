using System.Collections.Generic;

namespace ChessAvalonia.Models;

internal class GameState
{
    #region Constructors
    public GameState()
    {
        LocalPlayer = null;
        Opponent = null;
        MoveList = null;
        CurrentOnlineGame = null;
        IsOnlineGame = false;
        IsWaitingForMove = false;
        IsRotated = false;
        IsCheckMate = false;
    }
    #endregion

    #region Properties
    internal Player LocalPlayer { get; set; }
    internal Player Opponent { get; set; }
    internal List<Move> MoveList { get; set; }
    internal OnlineGame CurrentOnlineGame { get; set; }
    internal bool IsOnlineGame { get; set; }
    internal bool IsWaitingForMove { get; set; }
    internal bool IsRotated { get; set; }
    internal bool IsCheckMate { get; set; }
    #endregion

    #region Methods
    internal void Reset()
    {
        Opponent = null;
        IsWaitingForMove = false;
        IsOnlineGame = false;
        CurrentOnlineGame = null;
        IsCheckMate = false;
    }
    #endregion
}
