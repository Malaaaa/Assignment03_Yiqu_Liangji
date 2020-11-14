// declear the status in the game
public enum GameStatus
{
    Start = 0,  

    Pick,       
    Move,       
    Switch,     

    End,        
};

public enum GameSwitch
{
    White = -1, 

    Undefined,

    Black,      
};

public enum CoordType
{
    Pos = 0,    
    Vec = 1,    
};

public enum SquareType
{
    Undefined = 0,

    White,      
    Black,      
};

public enum SquareTint
{
    Undefined = 0,

    Pick,       
    Pass,       
    Kill,       
};

public class PieceName
{
    public const string WhiteCenterKing = "White Center King";
    public const string WhiteCenterQueen = "White Center Queen";
    public const string WhiteLeftBishop = "White Left Bishop";
    public const string WhiteRightBishop = "White Right Bishop";
    public const string WhiteLeftKnight = "White Left Knight";
    public const string WhiteRightKnight = "White Right Knight";
    public const string WhiteLeftRook = "White Left Rook";
    public const string WhiteRightRook = "White Right Rook";
    public const string WhitePawnA = "White Pawn A";
    public const string WhitePawnB = "White Pawn B";
    public const string WhitePawnC = "White Pawn C";
    public const string WhitePawnD = "White Pawn D";
    public const string WhitePawnE = "White Pawn E";
    public const string WhitePawnF = "White Pawn F";
    public const string WhitePawnG = "White Pawn G";
    public const string WhitePawnH = "White Pawn H";

    public const string BlackCenterKing = "Black Center King";
    public const string BlackCenterQueen = "Black Center Queen";
    public const string BlackLeftBishop = "Black Left Bishop";
    public const string BlackRightBishop = "Black Right Bishop";
    public const string BlackLeftKnight = "Black Left Knight";
    public const string BlackRightKnight = "Black Right Knight";
    public const string BlackLeftRook = "Black Left Rook";
    public const string BlackRightRook = "Black Right Rook";
    public const string BlackPawnA = "Black Pawn A";
    public const string BlackPawnB = "Black Pawn B";
    public const string BlackPawnC = "Black Pawn C";
    public const string BlackPawnD = "Black Pawn D";
    public const string BlackPawnE = "Black Pawn E";
    public const string BlackPawnF = "Black Pawn F";
    public const string BlackPawnG = "Black Pawn G";
    public const string BlackPawnH = "Black Pawn H";
};

public enum PieceType
{
    WhiteKing = 0,     
    WhiteQueen,         
    WhiteBishop,       
    WhiteKnight,        
    WhiteRook,          
    WhitePawn,         

    Undefined = 6,

    BlackKing,          
    BlackQueen,         
    BlackBishop,        
    BlackKnight,        
    BlackRook,          
    BlackPawn,          
};