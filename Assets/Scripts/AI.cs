using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    public static AI Instance = null;

    private static List<Square> squares = new List<Square>();
    private static List<Piece> pieces = new List<Piece>();
    List<Piece> Alivepieces = new List<Piece>();

    List<Piece> blackPieces = new List<Piece>();
    List<Piece> whitePieces = new List<Piece>();
    List<MoveData> _moves = new List<MoveData>();
    Stack<MoveData> moveStack = new Stack<MoveData>();
    Weights _weight = new Weights();
    MoveData bestMove;
    private GameObject selectedObject;


    int whiteScore = 0;
    int blackScore = 0;
    int maxDepth = 2;
    bool fakeLose = false;

    public int MaxDepth
    {
        get
        {
            return maxDepth;
        }
        set
        {
            maxDepth = value;
        }
    }
    public string AIcolor = "black";

    int _whiteScore = 0;
    int _blackScore = 0;
    Board _board = Board.Instance;
    GameController GameController;

    public MoveData GetMove()
    {
        _board = Board.Instance;
        squares =_board.squares;
        Debug.Log(squares.Count);


        GameController = GameController.Instance;
        bestMove = CreateMove(new Coordinate(0, 0), new Coordinate(0, 0));
        CalculateMinMax(maxDepth, int.MinValue, int.MaxValue, true);
        return bestMove;
    }

    int CalculateMinMax(int depth, int alpha, int beta, bool max)
    {
        _GetBoardState();

        if (depth == 0)
        {
            return _Evaluate();
        }
        if (max)
        {
            int score = -10000000;
            List<MoveData> allMoves = _GetMoves(AIcolor);
            Debug.Log(allMoves.Count);
            foreach (MoveData move in allMoves)
            {
                moveStack.Push(move);

                _DoFakeMove(move.firstPosition, move.secondPosition);

                score = CalculateMinMax(depth - 1, alpha, beta, false);

                _UndoFakeMove();

                if (score > alpha)
                {
                    Debug.Log(score);
                    Debug.Log(alpha);
                    Debug.Log(depth);
                    Debug.Log(maxDepth);
                    Debug.Log(move.score);
                    move.score = score;
                    if (move.score > bestMove.score && depth == maxDepth)
                    {
                        Debug.Log(depth);
                        bestMove = move;
                    }
                    alpha = score;
                }
                if (score >= beta)
                {
                    break;
                }
            }
            return alpha;
        }
        else
        {
            int score = 10000000;
            List<MoveData> allMoves = _GetMoves(AIcolor);
            foreach (MoveData move in allMoves)
            {
                moveStack.Push(move);
                Debug.Log(score);
                Debug.Log(beta);

                _DoFakeMove(move.firstPosition, move.secondPosition);

                score = CalculateMinMax(depth - 1, alpha, beta, true);

                _UndoFakeMove();

                if (score < beta)
                {
                    move.score = score;
                    beta = score;
                }
                Debug.Log(score);
                Debug.Log(beta);
                if (score <= alpha)
                {
                    break;
                }
            }
            return beta;
        }
    }

    void _UndoFakeMove()
    {
        MoveData tempMove = moveStack.Pop();
        Coordinate movedTo = tempMove.secondPosition;
        Coordinate movedFrom = tempMove.firstPosition;
        Piece pieceKilled = tempMove.pieceKilled;
        Piece pieceMoved = tempMove.pieceMoved;

        Piece CurrentPiece = _board.FindPiece(movedTo);
        CurrentPiece.coord = movedFrom;
        if (pieceKilled != null)
        {
            Piece piece = pieceKilled;
            piece.coord = movedTo;
        }
        else
        {
            movedTo = null;
        }
    }

    void _DoFakeMove(Coordinate fromTil, Coordinate targetTil)
    {

        Debug.Log("Select" + fromTil);
        Debug.Log("Target" + targetTil);
        Piece target = _board.FindPiece(targetTil);
        Piece from = _board.FindPiece(fromTil);

        if (target != null)
        {
            if ((target.type == PieceType.WhiteKing &&
                (int)from.type > 6) || (target.type == PieceType.BlackKing &&
                (int)from.type < 6))
            {
                Debug.Log("King is being targeted!");
                fakeLose = true;
            }
            else
            {
                fakeLose = false;
            }
            target = null;
        }
        from.coord = targetTil;
    }

    List<MoveData> _GetMoves(string color)
    {
        List<MoveData> turnMove = new List<MoveData>();
        List<Piece> pieces = new List<Piece>();

        if (color == "white")
            pieces = whitePieces;
        else pieces = blackPieces;
        Debug.Log(pieces.Count + "allili");
        foreach (Piece piece in pieces)
        {
            _board.pickPiece = piece;
            List<Coordinate> passCoords = _board.CheckNextSteps(piece);
            Debug.Log(passCoords.Count);
            Debug.Log(piece.coord);


            foreach (Coordinate cood in passCoords)
            {
                MoveData newMove = CreateMove(piece.coord, cood);
                turnMove.Add(newMove);
            }
            _board.pickPiece = null;
            passCoords.Clear();
        }
        return turnMove;
    }

    int _Evaluate()
    {
        float pieceDifference = 0;
        float whiteWeight = 0;
        float blackWeight = 0;

        foreach (Piece tile in whitePieces)
        {
            whiteWeight += _weight.GetBoardWeight(tile.type, tile.coord);
        }
        foreach (Piece tile in blackPieces)
        {
            blackWeight += _weight.GetBoardWeight(tile.type, tile.coord);
        }
        pieceDifference = (_blackScore + (blackWeight / 100)) - (_whiteScore + (whiteWeight / 100));
        return Mathf.RoundToInt(pieceDifference * 100);
    }

    void _GetBoardState()
    {
        blackPieces.Clear();
        whitePieces.Clear();
        blackScore = 0;
        whiteScore = 0;
        pieces.Clear();

        for(int i=0; i<64;i++)
        {
            Piece CurrentPiece = _board.FindPiece(squares[i].coord);
            Debug.Log(squares[i].go.name);
                      
     
            if (CurrentPiece != null)
            {
                {
                    pieces.Add(CurrentPiece);
                Debug.Log(CurrentPiece.go.name);          

                }
            }
        }
        foreach (Piece piece in pieces)
        {
            if ((int)piece.type > 6)
            {
                blackScore += _weight.GetPieceWeight(piece.type);
                blackPieces.Add(piece);
            }
            else
            {
                whiteScore += _weight.GetPieceWeight(piece.type);
                whitePieces.Add(piece);
            }
        }

    }

    MoveData CreateMove(Coordinate tile, Coordinate move)
    {
        MoveData tempMove = new MoveData
        {
            firstPosition = tile,
            pieceMoved = _board.FindPiece(tile),
            secondPosition = move
        };

        if (_board.FindPiece(move) != null)
        {
            tempMove.pieceKilled = _board.FindPiece(move);
        }

        return tempMove;
    }
}