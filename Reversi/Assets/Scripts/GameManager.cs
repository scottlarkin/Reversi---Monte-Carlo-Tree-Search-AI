using UnityEngine;
//using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

public class GameManager : MonoBehaviour {
	


    public Board board;
    private AIManager aiManager;
    private PlayerSettings playerSettings;
    private CameraManager cameraManager;
    private UIManager uiManager;
	
    public GameObject[][] pieceGameObjects;

    [HideInInspector] public GameObject black, white;

    private Point playerMove;

    void Awake()
    {
        aiManager = GameObject.Find("Managers").GetComponent<AIManager>();
        uiManager = Camera.main.GetComponent<UIManager>();
        playerSettings = Camera.main.GetComponent<PlayerSettings>();
        cameraManager = Camera.main.GetComponent<CameraManager>();

        board = GameObject.Find("Board").GetComponent<Board>();
        black = (GameObject)Resources.Load("Black");
        white = (GameObject)Resources.Load("White");

		board.state.turn = BoardState.GameTurn.Player;

        playerMove = null;
    }

    // Initializes board and camera, and starts main game loop
	IEnumerator Start()
    {
        // Gets player settings for board size
        yield return StartCoroutine(playerSettings.SettingsButton());
        
        InitializePieces();
        InstantiatePieces();
        
        cameraManager.SetCamera();
        
        StartCoroutine(GameLoop());
    }

    // Main game loop handling turns
    private IEnumerator GameLoop()
    {
        // Run game loop while the game isn't over (won/lost) or tied
		while (!board.state.GameOver && !board.state.GameTied)
        {
			board.state.turn = BoardState.GameTurn.Player;

            List<Point> PiecesToConvert_Player = new List<Point>();
			board.state.GenerateAvailableMoves();

			if (board.state.PossibleToMove)
            {
				board.state.WhiteCantMove = false;

                while (playerMove == null)
                {
                    yield return null;
                }
                // Clears the debug log
                // ClearLog();

                Debug.Log("Player's turn.");

				if (board.state.AvailableMove(playerMove, ref PiecesToConvert_Player))
                {
                    Debug.Log("Converting pieces from " + playerMove.X + "," + playerMove.Y);
                    ConvertPieces(PiecesToConvert_Player);
					Debug.Log("converted pieces from player");
                }
                else
                {
                    StartCoroutine(uiManager.ShowMessage("Invalid move.", 1));
                    playerMove = null;
                    continue;
                }
				//Debug.Log("Done player move");
            }
            else
            {
                Debug.Log("No possible player moves.");
                StartCoroutine(uiManager.ShowMessage("Player is unable to move.", 1));
				board.state.WhiteCantMove = true;
            }

	
			yield return new WaitForSeconds(0.5f);


            // Check whether game over before computer turn
			//Debug.Log ("checking game over");
			GameOver(board.state.CheckGameOver());

			if (!board.state.GameOver)
            {
                Debug.Log("Computer's turn.");
                List<Point> PiecesToConvert_Computer = new List<Point>();

				board.state.turn = BoardState.GameTurn.Computer;
				board.state.GenerateAvailableMoves();

                // If the computer can move, make the best possible move
				if (board.state.PossibleToMove)
                {
					board.state.BlackCantMove = false;

                    PiecesToConvert_Computer = aiManager.ComputerMoveMTCS();

                    Debug.Log("Converting pieces from " + PiecesToConvert_Computer[0].X + "," + PiecesToConvert_Computer[0].Y);
                    ConvertPieces(PiecesToConvert_Computer);

                }
                else
                {
                    Debug.Log("No possible computer moves.");
                    StartCoroutine(uiManager.ShowMessage("Computer is unable to move.", 1));
					board.state.BlackCantMove = true;
                }
            }
            // Check whether game over before player turn

			GameOver(board.state.CheckGameOver());

            playerMove = null;
        }

    }


	private void GameOver(int? GameState){
		if(GameState != null)
		{
			if(GameState != 0){
				uiManager.GameOver(board.state.WhiteCount, board.state.BlackCount);
			}
			else{
				uiManager.TieGame(board.state.WhiteCount, board.state.BlackCount);
			}
			enabled = !enabled;
		}
	}

    // Sets the player's move this turn
    public void SetInput(Point coord)
    {
        playerMove = coord;
    }

    // Converts board pieces following a move
    private void ConvertPieces(List<Point> PiecesToConvert)
    {


		board.state.ApplyMove(PiecesToConvert);

        foreach (Point point in PiecesToConvert)
        {
            GameObject temp = pieceGameObjects[point.X][point.Y];

            // Sets game object representation to instantiated piece
			pieceGameObjects[point.X][point.Y] = (board.state.turn.Equals(BoardState.GameTurn.Player)) ? (GameObject)Instantiate(white, board.WorldPosition(point.X, point.Y), white.transform.rotation)
                                                                                : (GameObject)Instantiate(black, board.WorldPosition(point.X, point.Y), black.transform.rotation);
			Destroy(temp);

        }

    }

    #region Piece initialization methods
    // Initializes the Piece[][] grid of pieces
    private void InitializePieces()
    {

		pieceGameObjects = new GameObject[board.state.Size][];

		for (int x = 0; x < board.state.Size; x++)
        {

			pieceGameObjects[x] = new GameObject[board.state.Size];

			for (int y = 0; y < board.state.Size; y++)
            {
				board.state.pieces[x][y] = BoardState.Piece.None;
                pieceGameObjects[x][y] = null;
            }
        }

		Point firstPiece = new Point((board.state.Size - 1) / 2, (board.state.Size - 1) / 2);

        // Sets first 4 pieces (2 Black, 2 White) in diagonal pattern
		board.state.pieces[firstPiece.X][firstPiece.Y] = BoardState.Piece.Black;
		board.state.pieces[firstPiece.X + 1][firstPiece.Y] = BoardState.Piece.White;
		board.state.pieces[firstPiece.X][firstPiece.Y + 1] = BoardState.Piece.White;
		board.state.pieces[firstPiece.X + 1][firstPiece.Y + 1] = BoardState.Piece.Black;
    }

    // Instantiates board pieces
    private void InstantiatePieces()
    {
		for (int x = 0; x < board.state.Size; x++)
        {
			for (int y = 0; y < board.state.Size; y++)
            {
				if (board.state.pieces[x][y] == BoardState.Piece.None)
                {
                    continue;
                }
				else if (board.state.pieces[x][y] == BoardState.Piece.Black)
                {
                    pieceGameObjects[x][y] = (GameObject)Instantiate(black, board.WorldPosition(x, y), black.transform.rotation);
					board.state.pieceCount++;
					board.state.BlackCount++;
                }
                else
                {
                    pieceGameObjects[x][y] = (GameObject)Instantiate(white, board.WorldPosition(x, y), white.transform.rotation);
					board.state.pieceCount++;
					board.state.WhiteCount++;
                }
            }
        }
    }
    #endregion
	
}







