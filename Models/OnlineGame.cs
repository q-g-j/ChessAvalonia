using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.Models;
internal class OnlineGame
{
    #region Constructors
    public OnlineGame()
    {
    }
    public OnlineGame(int whiteId, int blackId)
    {
        WhiteId = whiteId;
        BlackId = blackId;
    }
    #endregion

    #region Properties
    public int Id { get; set; }
    public int WhiteId { get; set; }
    public int BlackId { get; set; }
    public string LastMoveStart { get; set; }
    public string LastMoveEnd { get; set; }
    public bool PawnMovedTwoSquares { get; set; } = false;
    public char PromotePawnType { get; set; } = ' ';
    public bool IsCheckMate { get; set; } = false;
    public string MoveInfo { get; set; }
    public bool HasPlayerQuit { get; set; } = false;
    #endregion
}