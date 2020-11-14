using UnityEngine;

/*
 *  Declear the position for each sequare
 */
public class Coordinate
{
    public int[] pos = new int[2];              
    public Vector3 vec = Vector3.zero;         
    private const float transformers = 4.5f;    

    public Coordinate(int x, int y)
    {
        this.pos = new int[2] { x, y };
        vec = new Vector3(transformers - pos[0], 0, transformers - pos[1]);
    }

    public Coordinate(int[] pos)
    {
        this.pos = pos;
        vec = new Vector3(transformers - pos[0], 0, transformers - pos[1]);
    }

    public Coordinate(Vector3 vec)
    {
        this.vec = vec;
        pos[0] = (int)(transformers - vec.x);
        pos[1] = (int)(transformers - vec.z);
    }

    public void SetCoordinate(int[] pos)
    {
        this.pos = pos;
        vec = new Vector3(transformers - pos[0], 0, transformers - pos[1]);
    }

    public void SetCoordinate(Vector3 vec)
    {
        this.vec = vec;
        pos[0] = (int)(transformers - vec.x);
        pos[1] = (int)(transformers - vec.z);
    }

    public bool IsVaild()
    {
        if(pos[0] > 0 && pos[0] < 9 && pos[1] > 0 && pos[1] < 9)
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return (char)('A' + pos[0]-1) + "-" + (pos[1]);
    }
}
