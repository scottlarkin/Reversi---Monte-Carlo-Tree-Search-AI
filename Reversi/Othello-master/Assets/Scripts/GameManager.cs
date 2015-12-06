using UnityEngine;
//using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

public class GameManager : MonoBehaviour {

    [HideInInspector] public enum Piece { None, Black, White };
    [HideInInspector] public enum GameTurn { Player, Computer };

    private Board board;
    private AIManager aiManager;
    private PlayerSettings playerSettings;
    private CameraManager cameraManager;
    private UIManager uiManager;

    public Piece[][] pieces;
    public GameObject[][] pieceGameObjects;

    [HideInInspector] public GameObject black, white;

    private GameTurn turn;
    public GameTurn Turn { get { return turn; }}

    public bool WhiteCantMove { get; private set; }
    public bool BlackCantMove { get; private set; }

    public int Count, WhiteCount, BlackCount;

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

        turn = GameTurn.Player;

        Count = 0;
        WhiteCount = 0;
        BlackCount = 0;

        WhiteCantMove = false;
        BlackCantMove = false;

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
        while (!aiManager.GameOver && !aiManager.GameTied)
        {
            turn = GameTurn.Player;

            List<Point> PiecesToConvert_Player = new List<Point>();
            aiManager.GenerateAvailableMoves();

            // 
            if (aiManager.PossibleToMove)
            {
                WhiteCantMove = false;

                while (playerMove == null)
                {
                    yield return null;
                }
                // Clears the debug log
                // ClearLog();

                Debug.Log("Player's turn.");

                if (aiManager.AvailableMove(playerMove, ref PiecesToConvert_Player))
                {
                    Debug.Log("Converting pieces from " + playerMove.X + "," + playerMove.Y);
                    yield return StartCoroutine(ConvertPieces(PiecesToConvert_Player));
                }
                else
                {
                    StartCoroutine(uiManager.ShowMessage("Invalid move.", 1));
                    playerMove = null;
                    continue;
                }
            }
            else
            {
                Debug.Log("No possible player moves.");
                StartCoroutine(uiManager.ShowMessage("Player is unable to move.", 1));
                WhiteCantMove = true;
            }
			// Wait 0.5 seconds before computer moves for a more natural feel
			yield return new WaitForSeconds(0.5f);

            // Check whether game over before computer turn
            aiManager.CheckGameOver();

            if (!aiManager.GameOver)
            {
                Debug.Log("Computer's turn.");
                List<Point> PiecesToConvert_Computer = new List<Point>();

                turn = GameTurn.Computer;
                aiManager.GenerateAvailableMoves();

                // If the computer can move, make the best possible move
                if (aiManager.PossibleToMove)
                {
                    BlackCantMove = false;

                    PiecesToConvert_Computer = aiManager.ComputerMove();

                    Debug.Log("Converting pieces from " + PiecesToConvert_Computer[0].X + "," + PiecesToConvert_Computer[0].Y);
                    yield return StartCoroutine(ConvertPieces(PiecesToConvert_Computer));
                }
                else
                {
                    Debug.Log("No possible computer moves.");
                    StartCoroutine(uiManager.ShowMessage("Computer is unable to move.", 1));
                    BlackCantMove = true;
                }
            }
            // Check whether game over before player turn
            aiManager.CheckGameOver();

            playerMove = null;
        }
    }

    // Sets the player's move this turn
    public void SetInput(Point coord)
    {
        playerMove = coord;
    }

    // Converts board pieces following a move
    private IEnumerator ConvertPieces(List<Point> PiecesToConvert)
    {
        foreach (Point point in PiecesToConvert)
        {
            // Sets board square equal to piece
            pieces[point.X][point.Y] = (turn.Equals(GameTurn.Player)) ? Piece.White : Piece.Black;
            WhiteCount = (turn.Equals(GameTurn.Player)) ? WhiteCount + 1 : WhiteCount - 1;
            BlackCount = (turn.Equals(GameTurn.Computer)) ? BlackCount + 1 : BlackCount - 1;


            GameObject temp = pieceGameObjects[point.X][point.Y];

            // Sets game object representation to instantiated piece
            pieceGameObjects[point.X][point.Y] = (turn.Equals(GameTurn.Player)) ? (GameObject)Instantiate(white, board.WorldPosition(point.X, point.Y), white.transform.rotation)
                                                                                : (GameObject)Instantiate(black, board.WorldPosition(point.X, point.Y), black.transform.rotation);
            Destroy(temp);

            for (float timer = 0.1f; timer > 0; timer -= Time.deltaTime)
            {
                yield return null;
            }
        }

        // Accounts for subtracting original piece included in PiecesToConvert
        if (turn.Equals(GameTurn.Player))
        {
            BlackCount++;
        }
        else
        {
            WhiteCount++;
        }

        // Adds placed piece to the total count
        Count++;
    }

    #region Piece initialization methods
    // Initializes the Piece[][] grid of pieces
    private void InitializePieces()
    {
        pieces = new Piece[board.Size][];
        pieceGameObjects = new GameObject[board.Size][];

        for (int x = 0; x < board.Size; x++)
        {
            pieces[x] = new Piece[board.Size];
            pieceGameObjects[x] = new GameObject[board.Size];

            for (int y = 0; y < board.Size; y++)
            {
                pieces[x][y] = Piece.None;
                pieceGameObjects[x][y] = null;
            }
        }

        Point firstPiece = new Point((board.Size - 1) / 2, (board.Size - 1) / 2);

        // Sets first 4 pieces (2 Black, 2 White) in diagonal pattern
        pieces[firstPiece.X][firstPiece.Y] = Piece.Black;
        pieces[firstPiece.X + 1][firstPiece.Y] = Piece.White;
        pieces[firstPiece.X][firstPiece.Y + 1] = Piece.White;
        pieces[firstPiece.X + 1][firstPiece.Y + 1] = Piece.Black;
    }

    // Instantiates board pieces
    private void InstantiatePieces()
    {
        for (int x = 0; x < board.Size; x++)
        {
            for (int y = 0; y < board.Size; y++)
            {
                if (pieces[x][y] == Piece.None)
                {
                    continue;
                }
                else if (pieces[x][y] == Piece.Black)
                {
                    pieceGameObjects[x][y] = (GameObject)Instantiate(black, board.WorldPosition(x, y), black.transform.rotation);
                    Count++;
                    BlackCount++;
                }
                else
                {
                    pieceGameObjects[x][y] = (GameObject)Instantiate(white, board.WorldPosition(x, y), white.transform.rotation);
                    Count++;
                    WhiteCount++;
                }
            }
        }
    }
    #endregion

    /*
    #region Debug console methods
    // Clears the debug console
    public static void ClearLog()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(ActiveEditorTracker));

        System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
    #endregion
    */
}







