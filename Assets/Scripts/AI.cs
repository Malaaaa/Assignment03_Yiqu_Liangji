using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{

    List<Square> squares = new List<Square>();
    List<Piece> pieces = new List<Piece>();
    List<Piece> Alivepieces = new List<Piece>();

    List<Piece> blackPieces = new List<Piece>();
    List<Piece> whitePieces = new List<Piece>();
    Stack<MoveData> moveStack = new Stack<MoveData>();
    Weights weight = new Weights();
    MoveData bestMove;
    private GameObject selectedObject;
    public GameSwitch player;

    int whiteScore = 0;
    int blackScore = 0;
    int maxDepth = 2;

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
    Board board = Board.Instance;
    GameController GameController;
    public void setSquares()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                squares.Add(new Square(GameObject.Find("Square " + (char)('A' + j) + '-' + (i + 1))));
            }
        }
    }
    public MoveData GetMove(GameSwitch player)
    {
        board = Board.Instance;
        GameController = GameController.Instance;
        bestMove = CreateMove(new Coordinate(0, 0), new Coordinate(0, 0));
        CalculateMinMax(maxDepth, int.MinValue, int.MaxValue, true);
        Debug.Log(maxDepth);
        return bestMove;
    }

    int CalculateMinMax(int depth, int alpha, int beta, bool max)
    {
        GetBoardState();

        if (depth == 0)
        {
            return Evaluate();
        }
        if (max)
        {
            int score = -10000000;

            List<MoveData> allMoves = GetMoves(player);
            Debug.Log(allMoves.Count);

            foreach (MoveData move in allMoves)
            {
                moveStack.Push(move);

                DoFakeMove(move.firstPosition, move.secondPosition);

                score = CalculateMinMax(depth - 1, alpha, beta, false);
                Debug.Log(score);

                UndoFakeMove();

                if (score > alpha)
                {

                    Debug.Log(score);
                    Debug.Log(alpha);
                    Debug.Log(depth);
                    Debug.Log(move.score);

                    move.score = score;
                    if (move.score > bestMove.score && depth == maxDepth)
                    {
                        // Debug.Log(depth);
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
            List<MoveData> allMoves = GetMoves(GameSwitch.White);
            foreach (MoveData move in allMoves)
            {
                moveStack.Push(move);
                // Debug.Log(beta);

                DoFakeMove(move.firstPosition, move.secondPosition);

                score = CalculateMinMax(depth - 1, alpha, beta, true);

                UndoFakeMove();

                if (score < beta)
                {
                    move.score = score;
                    beta = score;
                }
                // Debug.Log(score);
                // Debug.Log(beta);
                if (score <= alpha)
                {
                    break;
                }
            }
            return beta;
        }
    }

    void UndoFakeMove()
    {
        MoveData tempMove = moveStack.Pop();
        Coordinate movedTo = tempMove.secondPosition;
        Coordinate movedFrom = tempMove.firstPosition;
        Piece pieceKilled = tempMove.pieceKilled;
        Piece pieceMoved = tempMove.pieceMoved;
        Piece CurrentPiece = board.FindPiece(movedTo);
        CurrentPiece.coord = movedFrom;
        if (pieceKilled != null)
        {
            Piece piece = pieceKilled;
        }
        else
        {
            movedTo = null;
        }

    }

    void DoFakeMove(Coordinate fromTil, Coordinate targetTil)
    {

        // Debug.Log("Select" + fromTil);
        // Debug.Log("Target" + targetTil);
        Piece target = board.FindPiece(targetTil);
        Piece from = board.FindPiece(fromTil);

        if (target != null)
        {
            Debug.Log("Target name"+target.go.name);
            target = null;
            Debug.Log("Target name"+target.go.name);
        }
        from.coord = targetTil;
    }

    List<MoveData> GetMoves(GameSwitch player)
    {
        List<MoveData> turnMove = new List<MoveData>();
        List<Piece> pieces = new List<Piece>();

        if (player == GameSwitch.White)
            pieces = whitePieces;
        else pieces = blackPieces;
        foreach (Piece piece in pieces)
        {
            board.pickPiece = piece;
            List<Coordinate> passCoords = board.CheckNextSteps(piece);
            foreach (Coordinate cood in passCoords)
            {
                MoveData newMove = CreateMove(piece.coord, cood);
                turnMove.Add(newMove);
            }
            board.pickPiece = null;
            passCoords.Clear();
        }
        return turnMove;
    }

    int Evaluate()
    {
        float pieceDifference = 0;
        float whiteWeight = 0;
        float blackWeight = 0;

        foreach (Piece tile in whitePieces)
        {
            whiteWeight += weight.GetBoardWeight(tile.type, tile.coord);
        }
        foreach (Piece tile in blackPieces)
        {
            blackWeight += weight.GetBoardWeight(tile.type, tile.coord);
        }
        pieceDifference = (blackScore + (blackWeight / 100)) - (whiteScore + (whiteWeight / 100));
        return Mathf.RoundToInt(pieceDifference * 100);
    }

    void GetBoardState()
    {
        blackPieces.Clear();
        whitePieces.Clear();
        blackScore = 0;
        whiteScore = 0;
        pieces.Clear();

        foreach(Square square in squares)
        {
            Piece CurrentPiece = board.FindPiece(square.coord);
            if (CurrentPiece != null)
            {
                {
                pieces.Add(CurrentPiece);
                }
            }
        }
        foreach (Piece piece in pieces)
        {
            if ((int)piece.type > 6)
            {
                blackScore += weight.GetPieceWeight(piece.type);
                blackPieces.Add(piece);
            }
            else
            {
                whiteScore += weight.GetPieceWeight(piece.type);
                whitePieces.Add(piece);
            }
        }

    }

    MoveData CreateMove(Coordinate piece, Coordinate move)
    {
        Piece pieceMoved = board.FindPiece(piece);
        Piece Target = board.FindPiece(move);
        MoveData tempMove = new MoveData
        {
            firstPosition = piece,
            pieceMoved = board.FindPiece(piece),
            secondPosition = move,
        };
        if (Target!= null)
        {
            if(((int)pieceMoved.type < 6 && (int)Target.type > 6) || ((int)pieceMoved.type > 6 && (int)Target.type < 6)){
                tempMove.pieceKilled = board.FindPiece(move);
                Debug.Log(tempMove.pieceKilled.go.name);
            }         
        }

        return tempMove;
    }
}