using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CanvasControllerScript : MonoBehaviour
{

    public GameObject Pause;

    public GameObject Background;

    private GameController controller;

    // Start is called before the first frame update
    void Start()
    {

        controller = GameController.Instance;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape") && !Background.active) {
            Pause.SetActive(true);
        } else
        {
            UIOperation();
        }
    }

    private GameObject PreCheckClick() {
    
        return EventSystem.current.currentSelectedGameObject;
    }

    private void UIOperation()
    {
        GameObject currentClickedUI = PreCheckClick();
        if (currentClickedUI != null)
        {
            string currentUIName = currentClickedUI.name;
            switch (currentUIName)
            {

                case "RandomPlayer":

                    //easy AI
                    // fake function
                    Background.SetActive(false);
                    controller.SetGameLevel(1);
                    break;

                case "HighDifficulty":

                    //Difficult AI
                    Background.SetActive(false);
                    controller.SetGameLevel(4);
                    break;

                case "Resume":

                    Pause.SetActive(false);
                    break;

                case "QuitGame":
                    Background.SetActive(true);
                    Pause.SetActive(false);
                    break;

                case "Quit":
                    Application.Quit();
                    break;
            }
        }
    }
}
