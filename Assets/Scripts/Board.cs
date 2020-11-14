using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Board 
{
    private int stepCount = 0;                                   
    private int stepSize = 0;                        

    private Animator animator = null;      
    private List<string> stepList = new List<string>();            

    private readonly string[] pieceStrArr = { "White Center King", "White Center Queen", "White Left Bishop", "White Right Bishop", 
                                              "White Left Knight", "White Right Knight", "White Left Rook", "White Right Rook", 
                                              "White Pawn A", "White Pawn B", "White Pawn C", "White Pawn D", 
                                              "White Pawn E", "White Pawn F", "White Pawn G", "White Pawn H", 
                                              "Black Center King", "Black Center Queen", "Black Left Bishop", "Black Right Bishop", 
                                              "Black Left Knight", "Black Right Knight", "Black Left Rook", "Black Right Rook", 
                                              "Black Pawn A", "Black Pawn B", "Black Pawn C", "Black Pawn D", 
                                              "Black Pawn E", "Black Pawn F", "Black Pawn G", "Black Pawn H" };

    public List<Square> squares = new List<Square>();              
    public List<Piece> pieces = new List<Piece>();                 
    public Piece pickPiece = null;                                  
    public Coordinate pickCoord = null;                             
    private List<Coordinate> passCoords = new List<Coordinate>();  
    private List<Coordinate> killCoords = new List<Coordinate>();   

    private static Board instance = null;
    public static Board Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Board();
            }
            return instance;
        }
    }

    
    /*
     *  Init the squares and the chess in the ground
     */
    public List<Square> InitSquares()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                squares.Add(new Square(GameObject.Find("Square " + (char)('A' + j) + '-' + (i + 1))));
            }
        }
        return squares;
    }

    // init the chess
    public void InitPieces()
    {
        foreach (string pieceStr in pieceStrArr)
        {
            pieces.Add(new Piece(GameObject.Find(pieceStr)));
        }
    }
    public void AddNav(){
        foreach(Piece piece in pieces)
        {
            NavMeshAgent nev =piece.go.AddComponent<NavMeshAgent>();
            nev.baseOffset= 0;
            nev.speed = 1f;
        }
    }
    // choose chess
    public GameObject PickPiece(Coordinate coord)
    {
        if (coord == null)
        {
            return null;
        }

        ClearSquares();

        pickCoord = coord;
        pickPiece = FindPiece(pickCoord);
        CheckNextSteps(pickPiece);

        TintSquares();
        return pickPiece.go;
    }

    // moving
    public GameObject MovePiece(Coordinate coord)
    {
        if (pickPiece != null)
        {
            NavMeshAgent agent = pickPiece.go.GetComponent<NavMeshAgent>();
            agent.SetDestination(coord.vec);
            animator = pickPiece.go.GetComponent<Animator>();
            animator.SetBool("Walking", true);
            pickPiece.coord = coord;
            pickPiece.step++;
            stepList.Add(++stepCount + "--" + pickPiece.go.name + "--" + pickPiece.coord.ToString());
        }
        ClearSquares();
        return pickPiece.go;
    }

    private void SetBoolAnimator(string paramName, bool isActive) {

        animator.SetBool(paramName, isActive);
    }

    // kill
    public void KillPiece(Coordinate coord)
    {
        Piece piece = FindPiece(coord);
        if (pieces.Remove(piece))
        {
            Object.Destroy(piece.go);
        }
    }

    /*
     *  path checking
     *  Thinking:
     *  Different chess have different rules
     *  Pawn, Bishop, King, Queen, Rook, Knight
     */ 
    public List<Coordinate> CheckNextSteps(Piece piece)
    {
        switch (piece.type)
        {
            case PieceType.WhitePawn:
            case PieceType.BlackPawn:
                {
                    bool canUp = true;
                    stepSize = (piece.step == 0) ? 3 : 2;
                    for (int i = 1; i < stepSize; i++)
                    {
                        if ((piece.type == PieceType.WhitePawn))
                        {
                            if (canUp && (canUp = CheckNextStep(piece.coord.pos[0], piece.coord.pos[1] + i, 1))) { }
                        }
                        else if ((piece.type == PieceType.BlackPawn))
                        {
                            if (canUp && (canUp = CheckNextStep(piece.coord.pos[0], piece.coord.pos[1] - i, 1))) { }
                        }
                    }
                    if ((piece.type == PieceType.WhitePawn))
                    {
                        CheckNextStep(piece.coord.pos[0] + 1, piece.coord.pos[1] + 1, 2);
                        CheckNextStep(piece.coord.pos[0] - 1, piece.coord.pos[1] + 1, 2);
                    }
                    else if ((piece.type == PieceType.BlackPawn))
                    {
                        CheckNextStep(piece.coord.pos[0] + 1, piece.coord.pos[1] - 1, 2);
                        CheckNextStep(piece.coord.pos[0] - 1, piece.coord.pos[1] - 1, 2);
                    }
                }
                break;
            case PieceType.WhiteRook:
            case PieceType.BlackRook:
                {
                    bool canUp = true, canRight = true, canBottom = true, canLeft = true;
                    stepSize = 8;
                    for (int i = 1; i < stepSize; i++)
                    {
                        if (canUp && (canUp = CheckNextStep(piece.coord.pos[0], piece.coord.pos[1] + i))) { }
                        if (canRight && (canRight = CheckNextStep(piece.coord.pos[0] + i, piece.coord.pos[1]))) { }
                        if (canBottom && (canBottom = CheckNextStep(piece.coord.pos[0], piece.coord.pos[1] - i))) { }
                        if (canLeft && (canLeft = CheckNextStep(piece.coord.pos[0] - i, piece.coord.pos[1]))) { }
                    }
                }
                break;
            case PieceType.WhiteKnight:
            case PieceType.BlackKnight:
                bool canUpRight = true, canRightUp = true, canRightBottom = true, canBottomRight = true,
                     canBottomLeft = true, canLeftBottom = true, canLeftUp = true, canUpLeft = true;
                if (canUpRight && (canUpRight = CheckNextStep(piece.coord.pos[0] + 1, piece.coord.pos[1] + 2))) { }
                if (canRightUp && (canRightUp = CheckNextStep(piece.coord.pos[0] + 2, piece.coord.pos[1] + 1))) { }
                if (canRightBottom && (canRightBottom = CheckNextStep(piece.coord.pos[0] + 2, piece.coord.pos[1] - 1))) { }
                if (canBottomRight && (canBottomRight = CheckNextStep(piece.coord.pos[0] + 1, piece.coord.pos[1] - 2))) { }
                if (canBottomLeft && (canBottomLeft = CheckNextStep(piece.coord.pos[0] - 1, piece.coord.pos[1] - 2))) { }
                if (canLeftBottom && (canLeftBottom = CheckNextStep(piece.coord.pos[0] - 2, piece.coord.pos[1] - 1))) { }
                if (canLeftUp && (canLeftUp = CheckNextStep(piece.coord.pos[0] - 2, piece.coord.pos[1] + 1))) { }
                if (canUpLeft && (canUpLeft = CheckNextStep(piece.coord.pos[0] - 1, piece.coord.pos[1] + 2))) { }
                break;
            case PieceType.WhiteBishop:
            case PieceType.BlackBishop:
                {
                    bool canNorthEast = true, canSouthEast = true, canSouthWest = true, canNorthWest = true;
                    stepSize = 8;
                    for (int i = 1; i < stepSize; i++)
                    {
                        if (canNorthEast && (canNorthEast = CheckNextStep(piece.coord.pos[0] + i, piece.coord.pos[1] + i))) { }
                        if (canSouthEast && (canSouthEast = CheckNextStep(piece.coord.pos[0] + i, piece.coord.pos[1] - i))) { }
                        if (canSouthWest && (canSouthWest = CheckNextStep(piece.coord.pos[0] - i, piece.coord.pos[1] - i))) { }
                        if (canNorthWest && (canNorthWest = CheckNextStep(piece.coord.pos[0] - i, piece.coord.pos[1] + i))) { }
                    }
                }
                break;
            case PieceType.WhiteQueen:
            case PieceType.BlackQueen:
                {
                    bool canUp = true, canRight = true, canBottom = true, canLeft = true,
                        canNorthEast = true, canSouthEast = true, canSouthWest = true, canNorthWest = true;
                    stepSize = 8;
                    for (int i = 1; i < stepSize; i++)
                    {
                        if (canUp && (canUp = CheckNextStep(piece.coord.pos[0], piece.coord.pos[1] + i))) { }
                        if (canRight && (canRight = CheckNextStep(piece.coord.pos[0] + i, piece.coord.pos[1]))) { }
                        if (canBottom && (canBottom = CheckNextStep(piece.coord.pos[0], piece.coord.pos[1] - i))) { }
                        if (canLeft && (canLeft = CheckNextStep(piece.coord.pos[0] - i, piece.coord.pos[1]))) { }
                        if (canNorthEast && (canNorthEast = CheckNextStep(piece.coord.pos[0] + i, piece.coord.pos[1] + i))) { }
                        if (canSouthEast && (canSouthEast = CheckNextStep(piece.coord.pos[0] + i, piece.coord.pos[1] - i))) { }
                        if (canSouthWest && (canSouthWest = CheckNextStep(piece.coord.pos[0] - i, piece.coord.pos[1] - i))) { }
                        if (canNorthWest && (canNorthWest = CheckNextStep(piece.coord.pos[0] - i, piece.coord.pos[1] + i))) { }
                    }
                }
                break;
            case PieceType.WhiteKing:
            case PieceType.BlackKing:
                {
                    bool canUp = true, canRight = true, canBottom = true, canLeft = true,
                        canNorthEast = true, canSouthEast = true, canSouthWest = true, canNorthWest = true;
                    if (canUp && (canUp = CheckNextStep(piece.coord.pos[0], piece.coord.pos[1] + 1))) { }
                    if (canRight && (canRight = CheckNextStep(piece.coord.pos[0] + 1, piece.coord.pos[1]))) { }
                    if (canBottom && (canBottom = CheckNextStep(piece.coord.pos[0], piece.coord.pos[1] - 1))) { }
                    if (canLeft && (canLeft = CheckNextStep(piece.coord.pos[0] - 1, piece.coord.pos[1]))) { }
                    if (canNorthEast && (canNorthEast = CheckNextStep(piece.coord.pos[0] + 1, piece.coord.pos[1] + 1))) { }
                    if (canSouthEast && (canSouthEast = CheckNextStep(piece.coord.pos[0] + 1, piece.coord.pos[1] - 1))) { }
                    if (canSouthWest && (canSouthWest = CheckNextStep(piece.coord.pos[0] - 1, piece.coord.pos[1] - 1))) { }
                    if (canNorthWest && (canNorthWest = CheckNextStep(piece.coord.pos[0] - 1, piece.coord.pos[1] + 1))) { }
                }
                break;
        }
        return passCoords;
    }

    // check the current chosed chess` nest step
    public bool CheckNextStep(int x, int y, int flag = 0)
    {
        Coordinate coord = new Coordinate(x, y);
        if (coord.IsVaild())
        {
            Piece piece = FindPiece(coord);
            if (piece == null && (flag == 0 || flag == 1))
            {
                passCoords.Add(coord);
            }
            else if (piece != null && piece.IsEnemy(pickPiece.type) && (flag == 0 || flag == 2))
            {
                killCoords.Add(coord);
                return false;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    // get square
    public Square FindSquare(Coordinate coord)
    {
        if (coord == null)
        {
            return null;
        }
        foreach (Square square in squares)
        {
            if (square.coord.pos[0] == coord.pos[0]
             && square.coord.pos[1] == coord.pos[1])
            {
                return square;
            }
        }
        return null;
    }

    // get chess
    public Piece FindPiece(Coordinate coord)
    {
        if (coord == null)
        {
            return null;
        }
        foreach (Piece piece in pieces)
        {
            if (piece.coord.pos[0] == coord.pos[0]
             && piece.coord.pos[1] == coord.pos[1])
            {
                return piece;
            }
        }
        return null;
    }

    /*
     *  Show the high light in the sequence
     */
    public void TintSquares()
    {
        if (pickPiece != null) 
        {
            TintSquare(pickPiece.coord, SquareTint.Pick);
        }
        foreach (Coordinate passCoord in passCoords) 
        {
            TintSquare(passCoord, SquareTint.Pass);
        }
        foreach (Coordinate killCoord in killCoords) 
        {
            TintSquare(killCoord, SquareTint.Kill);
        }
    }

    
    public bool TintSquare(Coordinate coord, SquareTint type)
    {
        if (coord == null)
        {
            return false;
        }
        Square square = null;
        if ((square = FindSquare(coord)) != null && square.go != null)
        {
            MeshRenderer renderer = null;
            if ((renderer = square.go.GetComponent<MeshRenderer>()) != null && renderer.material != null)
            {
                Color color = new Color32(223, 210, 192, 255);
                if (type == SquareTint.Pick)        // could be choose, be green
                {
                    color = new Color32(33, 150, 243, 255);
                }
                else if (type == SquareTint.Pass)   // could be move, be blue
                {
                    color = new Color32(76, 175, 80, 255);
                }
                else if (type == SquareTint.Kill)   // could be killed, be red
                {
                    color = new Color32(244, 67, 54, 255);
                }
                square.tint = type;
                renderer.material.color = color;
                return true;
            }
        }
        return false;
    }

    /*
     *  Clean the high light in the sequences
     */
    public void ClearSquares()
    {
        if (pickCoord != null)
        {
            ClearSquare(pickCoord);
        }
        foreach (Coordinate passCoord in passCoords)
        {
            ClearSquare(passCoord);
        }
        foreach (Coordinate killCoord in killCoords)
        {
            ClearSquare(killCoord);
        }
        pickCoord = null;
        passCoords.Clear();
        killCoords.Clear();
    }

    public bool ClearSquare(Coordinate coord)
    {
        if (coord == null)
        {
            return false;
        }
        Square square = null;
        if ((square = FindSquare(coord)) != null && square.go != null)
        {
            MeshRenderer renderer = null;
            if ((renderer = square.go.GetComponent<MeshRenderer>()) != null && renderer.material != null)
            {
                Color color = new Color32(223, 210, 192, 255);
                if (square.type == SquareType.White)
                {
                    color = new Color32(223, 210, 192, 255);
                }
                else if (square.type == SquareType.Black)
                {
                    color = new Color32(42, 40, 40, 255);
                }
                else
                {
                    return false;
                }
                square.tint = SquareTint.Undefined;
                renderer.material.color = color;
                return true;
            }
        }
        return false;
    }
}
