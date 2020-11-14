using UnityEngine;
using System.Collections;

public class MoveData
{
    public Coordinate firstPosition = null;
    public Coordinate secondPosition = null;
    public Piece pieceMoved = null;
    public Piece pieceKilled = null;
    public int score = int.MinValue;
}