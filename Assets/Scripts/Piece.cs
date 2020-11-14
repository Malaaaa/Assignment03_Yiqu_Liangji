using UnityEngine;
public class Piece
{   
    // chess gameobject
    public GameObject go;       
    public PieceType type;      
    public Coordinate coord;    
    public int step = 0;        

    public Piece(GameObject go)
    {
        this.go = go;
        type = NameToType(go.transform.name);
        coord = new Coordinate(go.transform.localPosition);
    }

    public GameObject GetGameObject() {

        return this.go;
    }

    /*
     *  Depending on the name, get the chess type
     */
    public PieceType NameToType(string name)
    {
        PieceType type = PieceType.Undefined;
        switch (name)
        {
            case PieceName.WhiteCenterKing:
                {
                    type = PieceType.WhiteKing;
                }
                break;
            case PieceName.WhiteCenterQueen:
                {
                    type = PieceType.WhiteQueen;
                }
                break;
            case PieceName.WhiteLeftBishop:
            case PieceName.WhiteRightBishop:
                {
                    type = PieceType.WhiteBishop;
                }
                break;
            case PieceName.WhiteLeftKnight:
            case PieceName.WhiteRightKnight:
                {
                    type = PieceType.WhiteKnight;
                }
                break;
            case PieceName.WhiteLeftRook:
            case PieceName.WhiteRightRook:
                {
                    type = PieceType.WhiteRook;
                }
                break;
            case PieceName.WhitePawnA:
            case PieceName.WhitePawnB:
            case PieceName.WhitePawnC:
            case PieceName.WhitePawnD:
            case PieceName.WhitePawnE:
            case PieceName.WhitePawnF:
            case PieceName.WhitePawnG:
            case PieceName.WhitePawnH:
                {
                    type = PieceType.WhitePawn;
                }
                break;
            case PieceName.BlackCenterKing:
                {
                    type = PieceType.BlackKing;
                }
                break;
            case PieceName.BlackCenterQueen:
                {
                    type = PieceType.BlackQueen;
                }
                break;
            case PieceName.BlackLeftBishop:
            case PieceName.BlackRightBishop:
                {
                    type = PieceType.BlackBishop;
                }
                break;
            case PieceName.BlackLeftKnight:
            case PieceName.BlackRightKnight:
                {
                    type = PieceType.BlackKnight;
                }
                break;
            case PieceName.BlackLeftRook:
            case PieceName.BlackRightRook:
                {
                    type = PieceType.BlackRook;
                }
                break;
            case PieceName.BlackPawnA:
            case PieceName.BlackPawnB:
            case PieceName.BlackPawnC:
            case PieceName.BlackPawnD:
            case PieceName.BlackPawnE:
            case PieceName.BlackPawnF:
            case PieceName.BlackPawnG:
            case PieceName.BlackPawnH:
                {
                    type = PieceType.BlackPawn;
                }
                break;
        }
        return type;
    }

    public bool IsEnemy(PieceType type)
    {
        if (((int)this.type < 6 && (int)type > 6) || ((int)this.type > 6 && (int)type < 6))
        {
            return true;
        }
        return false;
    }
}
