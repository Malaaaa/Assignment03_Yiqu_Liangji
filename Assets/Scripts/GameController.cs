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
    private int frame = 0;
    private GameStatus gameStatus;
    private GameSwitch gameSwitch;
    private Board board;
    private GameObject selectedObject;
    private GameObject chessBoard;

    private GameObject latestSelectedChess;

    private GameObject lastSelectSquare;

    private GameObject lastKilledChess;

    private AudioSource audioSource;

    void Start()
    {
        chessBoard = GameObject.FindGameObjectWithTag("Chess Game");
        gameStatus = GameStatus.Pick;
        gameSwitch = GameSwitch.White;
        board = new Board();
    }

    void Update()
    {
        if (!IsUIActive())
        {
            //RotateObjectToAngle(chessBoard, 0.25f);
            SingleRotate(chessBoard);
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
        }
        
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
    private void CheckAnimationStatus() {

        if (latestSelectedChess != null && lastSelectSquare != null) {
            
            if (CheckSamePosition(latestSelectedChess.transform.position, lastSelectSquare.transform.position)) {
                Animator chessAnimator = latestSelectedChess.GetComponent<Animator>();
                chessAnimator.SetBool("Walking", false);
            }
        }

        if (lastKilledChess != null) {
            float distance = Distance2TargetChess(latestSelectedChess, lastSelectSquare);
            Animator moveChessAnimator = latestSelectedChess.GetComponent<Animator>();
            Animator killedChessAnimator = lastKilledChess.GetComponent<Animator>();
            if (distance <= 1.5f && distance > 0.5f) {
                moveChessAnimator.SetBool("Attacking", true);
                killedChessAnimator.SetBool("Hurting", true);
                PlayAudioSource("Hurt", lastKilledChess.transform.position);
            } else if (distance <= 0.5f && distance > 0.3f) {
                killedChessAnimator.SetBool("Die", true);
                PlayAudioSource("Die", lastKilledChess.transform.position);
            } else if (distance <= 0.3f) {
                // moveChessAnimator.Play("Idle",0, 0);
                moveChessAnimator.SetBool("Attacking", false);
                moveChessAnimator.SetBool("Walking", false);
                Object.Destroy(lastKilledChess);
                lastKilledChess = null;
            }
        }
    }

    private bool CheckSamePosition(Vector3 position1, Vector3 position2) {
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
                lastKilledChess = board.FindPiece(selectedSquare.coord).go;
                PlayAudioSource("Attack", latestSelectedChess.transform.position);
                // board.KillPiece(selectedSquare.coord);
            }
            latestSelectedChess = board.MovePiece(selectedSquare.coord);
            PlayAudioSource("Move", latestSelectedChess.transform.position);
            gameStatus = GameStatus.Switch;
            gameSwitch = (gameSwitch == GameSwitch.White) ? GameSwitch.Black : GameSwitch.White;
        }
    }

    private float Distance2TargetChess(GameObject movedChess, GameObject killedChess) {

        Vector3 currentMovedChessPosition = movedChess.transform.position;
        Vector3 currentKilledChessPosition = killedChess.transform.position;
        Vector3 movePosition = new Vector3(currentMovedChessPosition.x, 0, currentMovedChessPosition.z);
        Vector3 killedPosition = new Vector3(currentKilledChessPosition.x, 0, currentKilledChessPosition.z);
        // Debug.Log("MovePosition: " + movePosition);
        // Debug.Log("KilledPosition: " + killedPosition);
        return Vector3.Distance(movePosition, killedPosition);
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
                Debug.Log(hitInfo.transform.name);
                Debug.DrawLine(ray.origin, hitInfo.point);
                // Debug.Log(hitInfo.transform.gameObject);
                return hitInfo.transform.gameObject;
            }
        }
        return null;
    }

    // 右键/单指拖拽旋转物体，每帧调用
    private void SingleRotate(GameObject gameObject)
    {
        if (Input.GetMouseButton(1) && gameObject.transform != null)
        {
            RotateObjectToAngle(gameObject, -Input.GetAxis("Mouse X") * Time.deltaTime * rotateFactory);
        }
    }

    // 顺时针旋转指定物体到指定角度
    private bool RotateObjectToAngle(GameObject gameObject, float angle)
    {
        if (gameObject != null)
        {
            gameObject.transform.Rotate(Vector3.up, angle, Space.World);
            return true;
        }
        else
        {
            return false;
        }
    }

    // 左键/单指拖拽指定物体
    private void SingleFingerDrag(GameObject gameObject)
    {
#if UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID
        // 移动端采用单指拖拽物体
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && gameObject.transform != null)
        {
            Vector2 position = Input.GetTouch(0).deltaPosition;
            gameObject.transform.Translate(-position.x * Time.deltaTime * dragFactory, -position.y * Time.deltaTime * dragFactory, 0);
        }
#elif UNITY_EDITOR
        // PC端采用左键拖拽旋转物体
        if (Input.GetMouseButton(0) && gameObject.transform != null)
        {
            DragObjectToPosition(gameObject, new Vector3(Input.GetAxis("Mouse X") * Time.deltaTime * dragFactory, Input.GetAxis("Mouse Y") * Time.deltaTime * dragFactory, 0));
        }
#endif  
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
    private void PlayAudioSource(string audioType, Vector3 playPoint)
    {
        if (IsAudioPlay())
        {
            audioSource.Stop();
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

    private bool IsAudioPlay()
    {
        return audioSource.isPlaying;
    }
}
