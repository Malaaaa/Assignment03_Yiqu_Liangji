using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardControllerScript : MonoBehaviour
{

    private RaycastHit raycastHit;
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
        
    }

    public bool RayShoot() {

        Ray rayCheck = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(rayCheck, out raycastHit, 100);
    }

    public RaycastHit GetRaycastHit() {

        return this.raycastHit;
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
			// Debug.Log(j + (i - 1) * 8);
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

}
