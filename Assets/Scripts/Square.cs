using UnityEngine;

public class Square
{
    public GameObject go;       
    public SquareType type;     
    public SquareTint tint;     
    public Coordinate coord;    

    public Square(GameObject go)
    {
        this.go = go;
        type = NameToType(go.transform.parent.transform.name);
        tint = SquareTint.Undefined;
        coord = new Coordinate(go.transform.localPosition);
    }

    public SquareType NameToType(string name)
    {
        if (name.Contains("White"))
        {
            return SquareType.White;
        }
        else if (name.Contains("Black"))
        {
            return SquareType.Black;
        }
        return SquareType.Undefined;
    }
}
