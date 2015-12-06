using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    private GUIStyle messageStyle;
    private GUIStyle buttonStyle;

    private bool drawGameOver;
    private bool drawMessage;

    private Vector2 buttonPosition;
    private Vector2 messagePosition;

    private float messageHeight;
    private float buttonHeight;

    private string message;

    void Awake()
    {
        messageHeight = Screen.height * .4f;
        buttonHeight = Screen.height * .4f;

        drawGameOver = false;
        drawMessage = false;
    }

    void Start()
    {
        buttonPosition.x = (Screen.width / 2.0f);
        buttonPosition.y = (Screen.height / 3.0f);

        messagePosition.x = buttonPosition.x;
        messagePosition.y = (Screen.height / 3f);
    }

    public IEnumerator ShowMessage(string message, int duration)
    {
        this.message = message;
        drawMessage = true;

        yield return new WaitForSeconds(duration);

        drawMessage = false;
    }

    public void OnGUI()
    {
        if (drawMessage && !drawGameOver)
        {
            messageStyle = new GUIStyle(GUI.skin.label);
            messageStyle.normal.textColor = Color.red;
            messageStyle.fontSize = 50;
            messageStyle.alignment = TextAnchor.UpperCenter;

            GUI.Label(new Rect(0, 0, Screen.width, messageHeight), message, messageStyle);
        }

        if (drawGameOver)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 30;
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            if(GUI.Button(new Rect(0, 0, Screen.width, buttonHeight), message, buttonStyle))
            {
                Application.LoadLevel("Othello");
            }
        }
    }

    public void TieGame(int whiteCount, int blackCount)
    {
        if (whiteCount > blackCount)
        {
            message = string.Format("No moves left, but you have more points. You win!\nScore: W {0} vs. B {1}\n\nNew game?", whiteCount, blackCount);
        }
        else
        {
            message = string.Format("No moves left, but you have fewer points. You lose!\nScore: W {0} vs. B {1}\n\nNew game?", whiteCount, blackCount);
        }

        drawGameOver = true;
    }

    public void GameOver(int whiteCount, int blackCount)
    {
        if (whiteCount > blackCount)
        {
            message = string.Format("You win!\nScore: W {0} vs. B {1}\n\nNew game?", whiteCount, blackCount);
        }
        else
        {
            message = string.Format("You lose!\nScore: W {0} vs. B {1}\n\nNew game?", whiteCount, blackCount);
        }

        drawGameOver = true;
    }
}





