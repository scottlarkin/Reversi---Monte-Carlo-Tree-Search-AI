using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BoardState
{
	[HideInInspector] public enum Piece { None, Black, White };
	[HideInInspector] public enum GameTurn { Player, Computer };
	
	public GameTurn turn;
	
	public Piece[][] pieces;
	
	public GameObject Tile;
	
	public int Size;
	
	public int Count;
	public int pieceCount, WhiteCount, BlackCount;

	public bool GameTied;
	public bool GameOver;
	
	public bool PossibleToMove;
	
	public bool WhiteCantMove;
	public bool BlackCantMove;

	public Dictionary<Point, List<Point>> availableMoves;

	public BoardState()
	{
		WhiteCantMove = false;
		BlackCantMove = false;

		Count = 0;

		pieceCount = 0;
		BlackCount = 0;
		WhiteCount = 0;
		// Dictionary that holds <Move, Points To Convert> 
		availableMoves = new Dictionary<Point, List<Point>>();
		
		GameOver = false;
		GameTied = false;
		PossibleToMove = false;
	}

	public void InitPieces()
	{
		pieces = new Piece[Size][];
		
		for(int i = 0; i <Size; i++)
		{
			pieces[i] = new Piece[Size];
		}
	}

	public BoardState(BoardState bs)
	{
		WhiteCantMove = false;
		BlackCantMove = false;
		turn = bs.turn;
		Size = bs.Size;

		Count = bs.Count;

		pieces = new Piece[Size][];
		
		for(int i = 0; i <Size; i++)
		{
			pieces[i] = new Piece[Size];

			for(int j = 0; j < Size; j++)
			{
				pieces[i][j] = bs.pieces[i][j];
			}
		}

		pieceCount = bs.pieceCount;
		BlackCount = bs.BlackCount;
		WhiteCount = bs.WhiteCount;
		// Dictionary that holds <Move, Points To Convert> 
		availableMoves = new Dictionary<Point, List<Point>>();
		
		GameOver = false;
		GameTied = false;
		PossibleToMove = bs.PossibleToMove;
		GenerateAvailableMoves();
	}

	public List<Point> PlacePiece(Point coord)
	{
		List<Point> changedPieces = new List<Point>();

		if(!availableMoves.TryGetValue(coord, out changedPieces))
		{
			Debug.Log("try get fail " + coord.X + "  " + coord.Y );
		}
				
		changedPieces.Insert(0, coord);
		
		return changedPieces ?? new List<Point>();
	}
	
	// Converts board pieces following a move
	public void ApplyMove(List<Point> PiecesToConvert)
	{

		if (PiecesToConvert.Count > 0) {
			foreach (Point point in PiecesToConvert) {
				WhiteCount = (turn.Equals (GameTurn.Player)) ? WhiteCount + 1 : WhiteCount - 1;
				BlackCount = (turn.Equals (GameTurn.Computer)) ? BlackCount + 1 : BlackCount - 1;
				
				pieces [point.X] [point.Y] = (turn.Equals (GameTurn.Player)) ? Piece.White : Piece.Black;
			}
			
			if (turn.Equals (GameTurn.Player)) {
				BlackCount++;
			} else {
				WhiteCount++;
			}
			// Adds placed piece to the total count
			pieceCount++;
		}

	}
	// Generates and stores all available moves for whoever's turn it is
	public void GenerateAvailableMoves()
	{
		availableMoves.Clear();
		PossibleToMove = false;
		
		for (int x = 0; x <  Size; x++)
		{
			for (int y = 0; y <  Size; y++)
			{
				if (pieces[x][y] == Piece.None)
				{
					List<Point> moves = new List<Point>();
					Point coord = new Point(x, y);
					
					AvailableUp(coord, ref moves);
					AvailableUpRight(coord, ref moves);
					AvailableRight(coord, ref moves);
					AvailableDownRight(coord, ref moves);
					AvailableDown(coord, ref moves);
					AvailableDownLeft(coord, ref moves);
					AvailableLeft(coord, ref moves);
					AvailableUpLeft(coord, ref moves);
					
					if (moves.Count > 0)
					{
						// If move converts at least 1 piece, add move to the dictionary of <Move, Points To Convert>
						availableMoves.Add(coord, moves);
						PossibleToMove = true;
					}
				}
			}
		}
		
	}
	
	// Returns whether or not move is available, and if so stores list of Points to convert
	public bool AvailableMove(Point coord, ref List<Point> PiecesToConvert)
	{
		if (availableMoves.TryGetValue(coord, out PiecesToConvert))
		{
			// Have to add the original move to the pieces to convert
			PiecesToConvert.Insert(0, coord);
			return true;
		}
		else
		{
			Debug.Log("Could not retrieve move " + coord.X + "," + coord.Y);
			return false;
		}
	}
	
	// Checks whether the game is over; i.e., lost/won or tied
	public int? CheckGameOver()
	{
		
		if ((WhiteCount == 0 || BlackCount == 0) || pieceCount == ( Size *  Size))
		{
			GameOver = true;
			
			//enabled = ! enabled;
			
			return WhiteCount > BlackCount ? 1 : -1;
		}
		
		if (WhiteCantMove && BlackCantMove)
		{
			GameTied = true;
			
			//enabled = ! enabled;
			
			return 0;
		}
		
		return null;
	}
	
	
	private void AvailableUp(Point coord, ref List<Point> PiecesToConvert)
	{
		Queue<Point> pieceQueue = new Queue<Point>();
		bool foundPlayerPiece = false;
		Piece pieceToCheck = (turn == GameTurn.Player) ? Piece.White : Piece.Black; 
		
		if (coord.Y !=  Size - 1)
		{
			for (int newY = coord.Y + 1; newY <  Size; newY++)
			{
				if (pieces[coord.X][newY].Equals(Piece.None))
				{
					return;
				}
				else if (pieces[coord.X][newY].Equals(pieceToCheck))
				{
					foundPlayerPiece = true;
					break;
				}
				else
				{
					pieceQueue.Enqueue(new Point(coord.X, newY));
				}
			}
			
			if (foundPlayerPiece)
			{
				while (pieceQueue.Count > 0)
				{
					PiecesToConvert.Add(pieceQueue.Dequeue());
				}
			}
		}
	}
	
	private void AvailableUpRight(Point coord, ref List<Point> PiecesToConvert)
	{
		Queue<Point> pieceQueue = new Queue<Point>();
		bool foundPlayerPiece = false;
		Piece pieceToCheck = (turn == GameTurn.Player) ? Piece.White : Piece.Black; 
		
		if (coord.X !=  Size -1 && coord.Y !=  Size - 1)
		{
			for (int newX = coord.X + 1, newY = coord.Y + 1; newX <  Size && newY <  Size; newX++, newY++)
			{
				if (pieces[newX][newY].Equals(Piece.None))
				{
					return;
				}
				else if (pieces[newX][newY].Equals(pieceToCheck))
				{
					foundPlayerPiece = true;
					break;
				}
				else
				{
					pieceQueue.Enqueue(new Point(newX, newY));
				}
			}
			
			if (foundPlayerPiece)
			{
				while (pieceQueue.Count > 0)
				{
					PiecesToConvert.Add(pieceQueue.Dequeue());
				}
			}
		}
	}
	
	private void AvailableRight(Point coord, ref List<Point> PiecesToConvert)
	{
		Queue<Point> pieceQueue = new Queue<Point>();
		bool foundPlayerPiece = false;
		Piece pieceToCheck = (turn == GameTurn.Player) ? Piece.White : Piece.Black; 
		
		if (coord.X !=  Size - 1)
		{
			for (int newX = coord.X + 1; newX <  Size; newX++)
			{
				if (pieces[newX][coord.Y].Equals(Piece.None))
				{
					return;
				}
				else if (pieces[newX][coord.Y].Equals(pieceToCheck))
				{
					foundPlayerPiece = true;
					break;
				}
				else
				{
					pieceQueue.Enqueue(new Point(newX, coord.Y));
				}
			}
			
			if (foundPlayerPiece)
			{
				while (pieceQueue.Count > 0)
				{
					PiecesToConvert.Add(pieceQueue.Dequeue());
				}
			}
		}
	}
	
	private void AvailableDownRight(Point coord, ref List<Point> PiecesToConvert)
	{
		Queue<Point> pieceQueue = new Queue<Point>();
		bool foundPlayerPiece = false;
		Piece pieceToCheck = (turn == GameTurn.Player) ? Piece.White : Piece.Black; 
		
		if (coord.X !=  Size - 1 && coord.Y != 0)
		{
			for (int newX = coord.X + 1, newY = coord.Y - 1; newX <  Size && newY >= 0; newX++, newY--)
			{
				if (pieces[newX][newY].Equals(Piece.None))
				{
					return;
				}
				else if (pieces[newX][newY].Equals(pieceToCheck))
				{
					foundPlayerPiece = true;
					break;
				}
				else
				{
					pieceQueue.Enqueue(new Point(newX, newY));
				}
			}
			
			if (foundPlayerPiece)
			{
				while (pieceQueue.Count > 0)
				{
					PiecesToConvert.Add(pieceQueue.Dequeue());
				}
			}
		}
	}
	
	private void AvailableDown(Point coord, ref List<Point> PiecesToConvert)
	{
		Queue<Point> pieceQueue = new Queue<Point>();
		bool foundPlayerPiece = false;
		Piece pieceToCheck = (turn == GameTurn.Player) ? Piece.White : Piece.Black; 
		
		if (coord.Y != 0)
		{
			for (int newY = coord.Y - 1; newY >= 0; newY--)
			{
				if (pieces[coord.X][newY].Equals(Piece.None))
				{
					return;
				}
				else if (pieces[coord.X][newY].Equals(pieceToCheck))
				{
					foundPlayerPiece = true;
					break;
				}
				else
				{
					pieceQueue.Enqueue(new Point(coord.X, newY));
				}
			}
			
			if (foundPlayerPiece)
			{
				while (pieceQueue.Count > 0)
				{
					PiecesToConvert.Add(pieceQueue.Dequeue());
				}
			}
		}
	}
	
	private void AvailableDownLeft(Point coord, ref List<Point> PiecesToConvert)
	{
		Queue<Point> pieceQueue = new Queue<Point>();
		bool foundPlayerPiece = false;
		Piece pieceToCheck = (turn == GameTurn.Player) ? Piece.White : Piece.Black; 
		
		if (coord.X != 0 && coord.Y != 0)
		{
			for (int newX = coord.X - 1, newY = coord.Y - 1; newX >= 0 && newY >= 0; newX--, newY--)
			{
				if (pieces[newX][newY].Equals(Piece.None))
				{
					return;
				}
				else if (pieces[newX][newY].Equals(pieceToCheck))
				{
					foundPlayerPiece = true;
					break;
				}
				else
				{
					pieceQueue.Enqueue(new Point(newX, newY));
				}
			}
			
			if (foundPlayerPiece)
			{
				while (pieceQueue.Count > 0)
				{
					PiecesToConvert.Add(pieceQueue.Dequeue());
				}
			}
		}
	}
	
	private void AvailableLeft(Point coord, ref List<Point> PiecesToConvert)
	{
		Queue<Point> pieceQueue = new Queue<Point>();
		bool foundPlayerPiece = false;
		Piece pieceToCheck = (turn == GameTurn.Player) ?Piece.White : Piece.Black; 
		
		if (coord.X != 0)
		{
			for (int newX = coord.X - 1; newX >= 0; newX--)
			{
				if (pieces[newX][coord.Y].Equals(Piece.None))
				{
					return;
				}
				else if (pieces[newX][coord.Y].Equals(pieceToCheck))
				{
					foundPlayerPiece = true;
					break;
				}
				else
				{
					pieceQueue.Enqueue(new Point(newX, coord.Y));
				}
			}
			
			if (foundPlayerPiece)
			{
				while (pieceQueue.Count > 0)
				{
					PiecesToConvert.Add(pieceQueue.Dequeue());
				}
			}
		}
	}
	
	private void AvailableUpLeft(Point coord, ref List<Point> PiecesToConvert)
	{
		Queue<Point> pieceQueue = new Queue<Point>();
		bool foundPlayerPiece = false;
		Piece pieceToCheck = (turn == GameTurn.Player) ? Piece.White : Piece.Black; 
		
		if (coord.X != 0 && coord.Y !=  Size - 1)
		{
			for (int newX = coord.X - 1, newY = coord.Y + 1; newX >= 0 && newY <  Size - 1; newX--, newY++)
			{
				if (pieces[newX][newY].Equals(Piece.None))
				{
					return;
				}
				else if (pieces[newX][newY].Equals(pieceToCheck))
				{
					foundPlayerPiece = true;
					break;
				}
				else
				{
					pieceQueue.Enqueue(new Point(newX, newY));
				}
			}
			
			if (foundPlayerPiece)
			{
				while (pieceQueue.Count > 0)
				{
					PiecesToConvert.Add(pieceQueue.Dequeue());
				}
			}
		}
	}
};

public class Board : MonoBehaviour {

	public BoardState state;

	public GameManager gameManager;
	public UIManager uiManager;

	public GameObject[][] board;
	
    public GameObject Tile;
	
    public float Offset { get; private set; }

    void Awake()
    {
		Offset = 0.05f;
		gameManager = GameObject.Find("Managers").GetComponent<GameManager>();
		uiManager = Camera.main.GetComponent<UIManager>();
		state = new BoardState();
	
    }

	// Sets board dimensions
	public void SetBoardDimensions(int boardSize)
    {
        Camera.main.GetComponent<CameraManager>().enabled = true;

		state.Size = boardSize;
		state.InitPieces();
        CreateBoard();
	}

    // Creates game board
    private void CreateBoard()
    {
        InitializeBoard();
        InstantiateBoard();
    }

    // Returns the position of the tile in the world
    public Vector3 WorldPosition(float x, float y)
    {
        float xCoord = (x == 0) ? x + .5f : x + .5f + (Offset * x);
        float yCoord = (y == 0) ? y + .5f : y + .5f + (Offset * y);
        
        return new Vector3(xCoord, yCoord, -1);
    }

    // Initializes the GameObject[][] with board tiles
    private void InitializeBoard()
    {
		board = new GameObject[state.Size][];

		for (int x = 0; x < state.Size; x++)
        {
			board[x] = new GameObject[state.Size];
            
			for (int y = 0; y < state.Size; y++)
            {
                board[x][y] = Tile;
			}
        }
    }

    // Instantiates the board tiles
    private void InstantiateBoard()
    {
		for (int x = 0; x < state.Size; x++)
        {
            float xCoord = (x == 0) ? x + .5f : x + .5f + (Offset * x);

			for (int y = 0; y < state.Size; y++)
            {
                float yCoord = (y == 0) ? y + .5f : y + .5f + (Offset * y);

                Vector3 position = new Vector3(xCoord, yCoord, 0);
				Instantiate(board[x][y], position, Quaternion.identity);
            }
        }
    }
}


