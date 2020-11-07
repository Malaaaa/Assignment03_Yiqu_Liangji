using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardControllerScript : MonoBehaviour
{

    private RaycastHit raycastHit;

    private bool isChoosed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Test, checking the grild number and choose object
        // if (RayShoot()) {
        //     Vector3 currentPosition = CalculateTheGrildPosition();
        //     if (IsValidPosition(currentPosition)) {
        //         int currentNumber = CalculateTheGrildNumber(currentPosition);
        //         Debug.Log("Number: " + currentNumber);
        //         Debug.Log(raycastHit.transform.tag);
        //     }
        // }

        // if (RayShoot()) {
        //     string currentBlockedTag = raycastHit.transform.tag;

        //     /*
        //      *  Status check. 
        //      *  When player choose a chess, set isChoosed = true;
        //      *  When player put a chess, set isChoosed = false;
        //      *  When player release a chess, set isChoosed = false;
        //      */ 
        //     if (Input.GetMouseButtonDown(0)) {

        //     } else if (Input.GetMouseButtonDown(1)) {
        //         // right click, release the chess to old position
        //     }
        //     // different chess have different rules
        //     Rules(currentBlockedTag);
            
        // }
        if (Input.GetMouseButtonDown(0)) {
            if (RayShoot()) {
                string currentTag = raycastHit.transform.tag;
                
                
            }
        }
    }

    public bool RayShoot() {

        Ray rayCheck = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(rayCheck, out raycastHit, 100);
    }

    public RaycastHit GetRaycastHit() {

        return this.raycastHit;
    }

    public bool IsChoosed() {

        return this.isChoosed;
    }

    /*
     *  Calculate the current mouse position in grild
     *  Rules function could use this, set it to public
     */
    public Vector3 CalculateTheGrildPosition() {

        if (RayShoot()) {
            Vector3 point = raycastHit.point;
            int i = (int) point.x;
            int j = (int) point.z;
            return new Vector3(i, 0, j);
        }
        return Vector3.zero;
    }

    /*
     *  To make sure the mouse position in the board
     */
    public bool IsValidPosition(Vector3 currentPosition) {

        if (currentPosition == Vector3.zero) {
            return false;
        }
        float x = currentPosition.x;
        float z = currentPosition.z;
        if (x > 8 || x < 0) {
            return false;
        }
        if (z > 8 || z < 0) {
            return false;
        }
        return true;
    }

    /*
     *  Calculate the position in which gril number
     *  Rules function could use this, set it to public
     */
    public int CalculateTheGrildNumber(Vector3 currentPosition) {

        float x = currentPosition.x;
        float z = currentPosition.z;
        return (int) (z + (x - 1) * 8);
    }

    /*
     *  Check the number whether in the board.
     */
    public bool IsValidNumber(int number) {
        
        return number >= 1 && number <= 64;
    }

    /*
     *  Predicte the position that the chess new position
     *  Use this function, could calculate the new possible positions 
     *  and show in the game
     */
    public List<Vector3> predicteTheFuturePosition() {


        return new List<Vector3>();
    }

    /*
     *  TO-DO discribe the rules for different chess
     */
    public void Rules(string objectTag) {

        switch (objectTag)
        {
            case "King" :

            break;

            case "Queen" :

            break;

            case "Knight" :

            break;

            case "Pawn" :

            break;

            case "Bishop" :

            break;

            case "Rook" :

            break;
        }

    }

}
