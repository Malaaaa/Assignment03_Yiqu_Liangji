using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Font font;
    private readonly float dragFactory = 0.5f;
    private int frame = 0;
    private GameStatus gameStatus;
    private GameSwitch gameSwitch;
    private Board board;
    private AI AI;
    private GameObject selectedObject;
    private GameObject chessBoard;

    private GameObject latestSelectedChess;

    private GameObject lastSelectSquare;
    private bool _kingDead = false;
    float timer = 0;

    public static GameController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }
    void Start()    
    {
        AI = new AI();

        chessBoard = GameObject.FindGameObjectWithTag("Chess Game");
        gameStatus = GameStatus.Pick;
        gameSwitch = GameSwitch.White;
        board = Board.Instance;
        board.InitPieces();
        board.InitSquares();

        board.AddNav();
    }

    void Update()
    {
        if (_kingDead)
        {
            Debug.Log("WINNER!");
            //UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
        if (!(gameSwitch == GameSwitch.White) && timer < 1)
        {
            timer += Time.deltaTime;
        }
        else if (!(gameSwitch == GameSwitch.White) && timer >= 1)
        {
            MoveData move = AI.GetMove();
            Debug.Log("KAishi "+move.secondPosition);
            _DoAIMove(move);
            timer = 0;
        }
        void _DoAIMove(MoveData move)
        {
        Coordinate firstPosition = move.firstPosition;
        Coordinate secondPosition = move.secondPosition;
        Piece firstPiece = board.FindPiece(firstPosition);
        Square Target = board.FindSquare(secondPosition);
        Debug.Log(firstPosition);
        SelectPiece(firstPiece.go);
        SelectPiece(Target.go);
    }
        //RotateObjectToAngle(chessBoard, 0.25f);
        if ((selectedObject = SingleClick()) != null)
        {
            if (gameStatus == GameStatus.Pick || gameStatus == GameStatus.Move)
            {
                SelectPiece(selectedObject);
            }
            if (gameStatus == GameStatus.Move)
            {
                SelectSquare(selectedObject);
            }
        }
        if (gameStatus == GameStatus.Switch)
        {
            SwitchGamer();
        }
        CleanChessStatus();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle
        {
            font = font,
            fontSize = 40,
            alignment = (TextAnchor)TextAlignment.Center,
            fontStyle = FontStyle.BoldAndItalic
        };
        style.normal.textColor = Color.white;
        if (gameStatus == GameStatus.End)
        {
            if (GUI.Button(new Rect(0, Screen.height * 0.1f, Screen.width, Screen.height * 0.1f), gameSwitch == GameSwitch.White ? "White Win" : "Black Win", style))
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
                gameStatus = GameStatus.Pick;
            }
        }
    }

    /*
     *  When Chess finished moving, should clean the animation status.
     *  Change it to "Idle"
     */
    private void CleanChessStatus() {

        if (latestSelectedChess != null && lastSelectSquare != null) {
            
            if (CheckSamePosition(latestSelectedChess.transform.position, lastSelectSquare.transform.position)) {
                Animator chessAnimator = latestSelectedChess.GetComponent<Animator>();
                chessAnimator.SetBool("Walking", false);
                chessAnimator.Play("Idle");
            }
        }
    }

    private bool CheckSamePosition(Vector3 position1, Vector3 position2) {
        
        return position1.x == position2.x && position1.z == position2.z;
    }

    private void    SelectPiece(GameObject selectedObject)
    {
        Piece selectedPiece = board.FindPiece(new Coordinate(selectedObject.transform.localPosition));
        if (selectedPiece != null)
        {
            if ((((int)selectedPiece.type < 6) && gameSwitch == GameSwitch.White) || (((int)selectedPiece.type > 6) && gameSwitch == GameSwitch.Black))
            {
                latestSelectedChess = board.PickPiece(selectedPiece.coord);
                gameStatus = GameStatus.Move;
            }
            else if (gameStatus == GameStatus.Move && ((((int)selectedPiece.type > 6) && gameSwitch == GameSwitch.White) || (((int)selectedPiece.type < 6) && gameSwitch == GameSwitch.Black)))
            {
                SelectSquare(selectedObject);
            }
        }
    }

    private void SelectSquare(GameObject selectedObject)
    {
        Square selectedSquare = board.FindSquare(new Coordinate(selectedObject.transform.localPosition));
        if (selectedSquare != null && (selectedSquare.tint == SquareTint.Pass || selectedSquare.tint == SquareTint.Kill))
        {
            lastSelectSquare = selectedSquare.go;
            if (selectedSquare.tint == SquareTint.Kill)
            {
                if (board.FindPiece(selectedSquare.coord).type == PieceType.BlackKing || board.FindPiece(selectedSquare.coord).type == PieceType.WhiteKing)
                {
                    gameStatus = GameStatus.End;
                }
                GameObject killedChess = board.FindPiece(selectedSquare.coord).go;
                // TODO
                Debug.Log(lastSelectSquare);
                Debug.Log(latestSelectedChess);
                Debug.Log(killedChess);
                board.KillPiece(selectedSquare.coord);
            }
            latestSelectedChess = board.MovePiece(selectedSquare.coord);
            gameStatus = GameStatus.Switch;
            gameSwitch = (gameSwitch == GameSwitch.White) ? GameSwitch.Black : GameSwitch.White;
        }
    }

    private void SwitchGamer()
    {
        if (frame < 45)
        {
            frame++;
        }
        else
        {
            gameStatus = GameStatus.Pick;
            frame = 0;
        }
    }

    // 左键/单指点击物体，返回物体名
    private GameObject SingleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                Debug.DrawLine(ray.origin, hitInfo.point);
                Debug.Log(hitInfo.transform.gameObject);
                return hitInfo.transform.gameObject;
            }
        }
        return null;
    }

    // 右键/单指拖拽旋转物体，每帧调用

    // 顺时针旋转指定物体到指定角度


    // 左键/单指拖拽指定物体
    private void SingleFingerDrag(GameObject gameObject)
    {
        // PC端采用左键拖拽旋转物体
        if (Input.GetMouseButton(0) && gameObject.transform != null)
        {
            DragObjectToPosition(gameObject, new Vector3(Input.GetAxis("Mouse X") * Time.deltaTime * dragFactory, Input.GetAxis("Mouse Y") * Time.deltaTime * dragFactory, 0));
        }
    }

    // 拖拽指定物体到指定位置
    private bool DragObjectToPosition(GameObject gameObject, Vector3 position)
    {
        if (gameObject != null)
        {
            gameObject.transform.Translate(position);
            return true;
        }
        else
        {
            return false;
        }
    }
}
