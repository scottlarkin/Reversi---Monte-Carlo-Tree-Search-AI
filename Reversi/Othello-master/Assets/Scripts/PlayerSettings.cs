using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class PlayerSettings : MonoBehaviour {

    private Board board;

    private GUIStyle boxStyle;
    private GUIStyle buttonStyle;

    private Vector2 buttonPosition;
    private float buttonWidth;
    private float buttonHeight;

    private float boxMessageOffset;

    private int boardSize;

    // Setting strings
    private string menu = "Board size";
    private string four = "4x4";
    private string eight = "8x8";
    private string sixteen = "16x16";
    private string thirtytwo = "32x32";

    void Awake()
    {
        GUI.enabled = false;
        board = GameObject.Find("Board").GetComponent<Board>();

        buttonWidth = Screen.width * .1f;
        buttonHeight = Screen.height * .05f;
        boxMessageOffset = 50f;

        boardSize = -1;

        buttonPosition.x = (Screen.width / 2.0f) - (buttonWidth / 2);
        buttonPosition.y = (Screen.height / 2.0f) - (buttonHeight / 2);
    }

    void OnGUI()
    {
        // GUI.Box that frames all settings buttons in the middle of the screen
        boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = 30;
        GUI.Box(new Rect(buttonPosition.x - (buttonWidth * 1.5f), buttonPosition.y - boxMessageOffset, buttonWidth * 4, buttonHeight + boxMessageOffset), menu, boxStyle);

        // GUI.Buttons that represent the available board sizes: 4x4, 8x8, 16x16, 32x32
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 16;

        if (GUI.Button(new Rect(buttonPosition.x - (buttonWidth * 1.5f), buttonPosition.y, buttonWidth, buttonHeight), four, buttonStyle))
        {
            boardSize = 4;
        }
        else if (GUI.Button(new Rect(buttonPosition.x - (buttonWidth * .5f), buttonPosition.y, buttonWidth, buttonHeight), eight, buttonStyle))
        {
            boardSize = 8;
        }
        else if (GUI.Button(new Rect(buttonPosition.x + (buttonWidth * .5f), buttonPosition.y, buttonWidth, buttonHeight), sixteen, buttonStyle))
        {
            boardSize = 16;
        }
        else if (GUI.Button(new Rect(buttonPosition.x + (buttonWidth * 1.5f), buttonPosition.y, buttonWidth, buttonHeight), thirtytwo, buttonStyle))
        {
            boardSize = 32;
        }
    }

    // Waits for player to choose board size and then sets board size
    public IEnumerator SettingsButton()
    {
        GUI.enabled = true;

        while (true)
        {
            if (boardSize != -1)
            {
                board.SetBoardDimensions(boardSize);
                break;
            }
            else
            {
                yield return null;
            }
        }
        
        Debug.Log("Set board size to " + boardSize + "," + boardSize);
        this.enabled = false;
    }
}
