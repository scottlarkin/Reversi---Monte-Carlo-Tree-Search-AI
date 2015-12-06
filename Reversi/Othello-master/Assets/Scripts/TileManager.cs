using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour {

    private Color tileColor;
    private Board board;
    private Point coord;

    private GameManager gameManager;
    private AIManager aiManager;

    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        gameManager = GameObject.Find("Managers").GetComponent<GameManager>();
        aiManager = GameObject.Find("Managers").GetComponent<AIManager>();
    }

    void Start()
    {
        tileColor = renderer.material.color;
        board.Count++;

        coord = new Point((board.Count - 1) / board.Size, (board.Count - 1) % board.Size);
    }
	
    #region Mouse input methods
    void OnMouseEnter()
    {
        if (!aiManager.GameOver)
        {
            // Highlights the tile as yellow
            renderer.material.color = Color.yellow;
        }
    }

    void OnMouseExit()
    {
        // Sets the tile back to its original color
        renderer.material.color = tileColor;
    }

    void OnMouseUpAsButton()
    {
        if (gameManager.Turn == GameManager.GameTurn.Player && !aiManager.GameOver)
        {
            gameManager.SetInput(coord);
        }
    }
    #endregion Mouse input methods
}
