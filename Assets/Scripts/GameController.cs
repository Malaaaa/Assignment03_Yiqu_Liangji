using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Font font;
    public AudioClip moving;
    public AudioClip attack;
    public AudioClip die;
    public AudioClip hurt;
    private readonly float rotateFactory = 100f;
    private readonly float dragFactory = 0.5f;
    private float HoldingSpeed = 100f;
    private float movingSpeed = 1f;
    private int frame = 0;
    private GameStatus gameStatus;
    public GameSwitch gameSwitch;
    private Board board;
    private AI AI;
    private GameObject selectedObject;
    private GameObject chessBoard;
    private GameObject latestSelectedChess;
    private GameObject lastSelectSquare;
    private GameObject lastKilledChess;
    private AudioSource audioSource;
    private bool _kingDead = false;
    private int GameLevel = 0;
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
        AI.setSquares();
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
            // Debug.Log("WINNER!");
            //UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
        if (gameSwitch == GameSwitch.Black && timer < 3)
        {
            timer += Time.deltaTime;
        }
        else if (gameSwitch == GameSwitch.Black && timer >= 3)
        {
            MoveData move = AI.GetMove();
            // Debug.Log("KAishi " + move.firstPosition);
            _DoAIMove(move);
            timer = 0;
        }
        void _DoAIMove(MoveData move)
        {
            Coordinate firstPosition = move.firstPosition;
            Coordinate secondPosition = move.secondPosition;
            Piece firstPiece = board.FindPiece(firstPosition);
            Square Target = board.FindSquare(secondPosition);
            // Debug.Log(firstPosition);
            SelectPiece(firstPiece.go);
            SelectSquare(Target.go);
        }
        
        if (!IsUIActive())
        {
            board.SetMovingSpeed(movingSpeed);
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
            CheckAnimationStatus();
        } else {
            board.SetMovingSpeed(HoldingSpeed);
        }
        // if (gameStatus == GameStatus.Switch)
        // {
        //     SwitchGamer();
        // }
        // CheckAnimationStatus();
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
    private void CheckAnimationStatus()
    {
        float speed = movingSpeed;
        if (IsUIActive()) {
            speed = HoldingSpeed;
        }
        if (latestSelectedChess != null && lastSelectSquare != null)
        {

            if (CheckSamePosition(latestSelectedChess.transform.position, lastSelectSquare.transform.position))
            {
                Animator chessAnimator = latestSelectedChess.GetComponent<Animator>();
                chessAnimator.speed = speed;
                chessAnimator.SetBool("Attacking", false);
                chessAnimator.SetBool("Walking", false);
                chessAnimator.Play("Idle");
            }
        }

        if (lastKilledChess != null)
        {
            float distance = Distance2TargetChess(latestSelectedChess, lastSelectSquare);
            Animator moveChessAnimator = latestSelectedChess.GetComponent<Animator>();
            Animator killedChessAnimator = lastKilledChess.GetComponent<Animator>();
            moveChessAnimator.speed = speed;
            killedChessAnimator.speed = speed;
            if (distance <= 1.5f && distance > 0.5f)
            {
                Debug.Log("Attack set true");
                moveChessAnimator.SetBool("Attacking", true);
                killedChessAnimator.SetBool("Hurting", true);
            }
            else if (distance <= 0.5f && distance > 0.3f)
            {
                Debug.Log("Attack set false");
                killedChessAnimator.SetBool("Die", true);
                moveChessAnimator.SetBool("Attacking", false);
                PlayAudioSource("Die", lastKilledChess.transform.position);
            }
            else if (distance <= 0.3f)
            {
                // moveChessAnimator.SetBool("Attacking", false);
                // moveChessAnimator.Play("Idle",0, 0);
                Debug.Log("Walking set false");
                moveChessAnimator.SetBool("Walking", false);
                moveChessAnimator.Play("Idle");
                Object.Destroy(lastKilledChess);
                lastKilledChess = null;
            }
        }
    }

    private float Distance2TargetChess(GameObject movedChess, GameObject killedChess) {

        Vector3 currentMovedChessPosition = movedChess.transform.position;
        Vector3 currentKilledChessPosition = killedChess.transform.position;
        Vector3 movePosition = new Vector3(currentMovedChessPosition.x, 0, currentMovedChessPosition.z);
        Vector3 killedPosition = new Vector3(currentKilledChessPosition.x, 0, currentKilledChessPosition.z);
        return Vector3.Distance(movePosition, killedPosition);
    }


    private bool CheckSamePosition(Vector3 position1, Vector3 position2)
    {
        return position1.x == position2.x && position1.z == position2.z;
    }

    private void SelectPiece(GameObject selectedObject)
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
                lastKilledChess = board.FindPiece(selectedSquare.coord).go;
                PlayAudioSource("Attack", latestSelectedChess.transform.position);
                PlayAudioSource("Hurt", lastKilledChess.transform.position);
                // board.KillPiece(selectedSquare.coord);
            }
            latestSelectedChess = board.MovePiece(selectedSquare.coord);
            PlayAudioSource("Move", latestSelectedChess.transform.position);
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

    private GameObject SingleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {  
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                // Debug.Log(hitInfo.transform.name);
                // Debug.DrawLine(ray.origin, hitInfo.point);
                // Debug.Log(hitInfo.transform.gameObject);
                return hitInfo.transform.gameObject;
            }
        }
        return null;
    }

    private bool IsUIActive()
    {
        GameObject[] UIPanels = GameObject.FindGameObjectsWithTag("UI");
        if (UIPanels.Length > 0)
        {
            return true;
        }
        return false;
    }

    /*
     * Play audio by type
     */
    private void PlayAudioSource(string audioType, Vector3 playPoint){

        // if (IsAudioPlay())
        // {
        //     audioSource.Stop();
        // }
        if (IsUIActive()) {
            return ;
        }
        switch (audioType)
        {
            case "Attack":
                AudioSource.PlayClipAtPoint(attack, playPoint);
                break;

            case "Die":
                AudioSource.PlayClipAtPoint(die, playPoint);
                break;

            case "Hurt":
                AudioSource.PlayClipAtPoint(hurt, playPoint);
                break;

            case "Move":
                AudioSource.PlayClipAtPoint(moving, playPoint);
                break;
        }

    }

    public void SetLevel(int level) {

        this.GameLevel = level;
    }

}
