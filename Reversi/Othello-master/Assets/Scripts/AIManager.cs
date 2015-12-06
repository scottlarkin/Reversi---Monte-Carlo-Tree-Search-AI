using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour {

    private GameManager gameManager;
    private Board board;
    private UIManager uiManager;

    private Dictionary<Point, List<Point>> availableMoves;

    public bool GameTied { get; private set; }
    public bool GameOver { get; private set; }

    public bool PossibleToMove { get; private set; }

	void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        gameManager = GameObject.Find("Managers").GetComponent<GameManager>();
        uiManager = Camera.main.GetComponent<UIManager>();

        // Dictionary that holds <Move, Points To Convert> 
        availableMoves = new Dictionary<Point, List<Point>>();

        GameOver = false;
        GameTied = false;
        PossibleToMove = false;
	}

    // Generates and stores all available moves for whoever's turn it is
    public void GenerateAvailableMoves()
    {
        availableMoves.Clear();
        PossibleToMove = false;

        for (int x = 0; x < board.Size; x++)
        {
            for (int y = 0; y < board.Size; y++)
            {
                if (gameManager.pieces[x][y] == GameManager.Piece.None)
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

    // Returns the best possible computer move as a List<Point>, including the piece put down
    public List<Point> ComputerMove()
    {
        List<Point> computerMove = new List<Point>();
        Point bestMove = new Point();

        int mostPoints = 0;

        foreach (KeyValuePair<Point, List<Point>> move in availableMoves)
        {
            if (move.Value.Count > mostPoints)
            {
                mostPoints = move.Value.Count;
                bestMove = move.Key;
            }
            // 50% chance to use an equally-viable move in order to shift the physical move distribution
            else if (move.Value.Count == mostPoints)
            {
                if (UnityEngine.Random.Range(0, 9) <= 4)
                {
                    mostPoints = move.Value.Count;
                    bestMove = move.Key;
                }
            }
        }

        availableMoves.TryGetValue(bestMove, out computerMove);
        // Have to add the move itself to the list of Points
        computerMove.Insert(0, bestMove);

        return computerMove;
    }

    // Checks whether the game is over; i.e., lost/won or tied
    public void CheckGameOver()
    {
        if (gameManager.Count == (board.Size * board.Size))
        {
            GameOver = true;

            board.enabled = !board.enabled;
            gameManager.enabled = !gameManager.enabled;

            uiManager.GameOver(gameManager.WhiteCount, gameManager.BlackCount);
        }

        if (gameManager.WhiteCount == 0 || gameManager.BlackCount == 0)
        {
            GameOver = true;

            board.enabled = !board.enabled;
            gameManager.enabled = !gameManager.enabled;

            uiManager.GameOver(gameManager.WhiteCount, gameManager.BlackCount);
        }

        if (gameManager.WhiteCantMove && gameManager.BlackCantMove)
        {
            GameTied = true;

            board.enabled = !board.enabled;
            gameManager.enabled = !gameManager.enabled;

            uiManager.TieGame(gameManager.WhiteCount, gameManager.BlackCount);
        }
    }
    
    #region Available move helper methods
    private void AvailableUp(Point coord, ref List<Point> PiecesToConvert)
    {
        Queue<Point> pieceQueue = new Queue<Point>();
        bool foundPlayerPiece = false;
        GameManager.Piece pieceToCheck = (gameManager.Turn == GameManager.GameTurn.Player) ? GameManager.Piece.White : GameManager.Piece.Black; 
        
        if (coord.Y != board.Size - 1)
        {
            for (int newY = coord.Y + 1; newY < board.Size; newY++)
            {
                if (gameManager.pieces[coord.X][newY].Equals(GameManager.Piece.None))
                {
                    return;
                }
                else if (gameManager.pieces[coord.X][newY].Equals(pieceToCheck))
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
        GameManager.Piece pieceToCheck = (gameManager.Turn == GameManager.GameTurn.Player) ? GameManager.Piece.White : GameManager.Piece.Black; 
        
        if (coord.X != board.Size -1 && coord.Y != board.Size - 1)
        {
            for (int newX = coord.X + 1, newY = coord.Y + 1; newX < board.Size && newY < board.Size; newX++, newY++)
            {
                if (gameManager.pieces[newX][newY].Equals(GameManager.Piece.None))
                {
                    return;
                }
                else if (gameManager.pieces[newX][newY].Equals(pieceToCheck))
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
        GameManager.Piece pieceToCheck = (gameManager.Turn == GameManager.GameTurn.Player) ? GameManager.Piece.White : GameManager.Piece.Black; 
        
        if (coord.X != board.Size - 1)
        {
            for (int newX = coord.X + 1; newX < board.Size; newX++)
            {
                if (gameManager.pieces[newX][coord.Y].Equals(GameManager.Piece.None))
                {
                    return;
                }
                else if (gameManager.pieces[newX][coord.Y].Equals(pieceToCheck))
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
        GameManager.Piece pieceToCheck = (gameManager.Turn == GameManager.GameTurn.Player) ? GameManager.Piece.White : GameManager.Piece.Black; 
        
        if (coord.X != board.Size - 1 && coord.Y != 0)
        {
            for (int newX = coord.X + 1, newY = coord.Y - 1; newX < board.Size && newY >= 0; newX++, newY--)
            {
                if (gameManager.pieces[newX][newY].Equals(GameManager.Piece.None))
                {
                    return;
                }
                else if (gameManager.pieces[newX][newY].Equals(pieceToCheck))
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
        GameManager.Piece pieceToCheck = (gameManager.Turn == GameManager.GameTurn.Player) ? GameManager.Piece.White : GameManager.Piece.Black; 
        
        if (coord.Y != 0)
        {
            for (int newY = coord.Y - 1; newY >= 0; newY--)
            {
                if (gameManager.pieces[coord.X][newY].Equals(GameManager.Piece.None))
                {
                    return;
                }
                else if (gameManager.pieces[coord.X][newY].Equals(pieceToCheck))
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
        GameManager.Piece pieceToCheck = (gameManager.Turn == GameManager.GameTurn.Player) ? GameManager.Piece.White : GameManager.Piece.Black; 
        
        if (coord.X != 0 && coord.Y != 0)
        {
            for (int newX = coord.X - 1, newY = coord.Y - 1; newX >= 0 && newY >= 0; newX--, newY--)
            {
                if (gameManager.pieces[newX][newY].Equals(GameManager.Piece.None))
                {
                    return;
                }
                else if (gameManager.pieces[newX][newY].Equals(pieceToCheck))
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
        GameManager.Piece pieceToCheck = (gameManager.Turn == GameManager.GameTurn.Player) ? GameManager.Piece.White : GameManager.Piece.Black; 
        
        if (coord.X != 0)
        {
            for (int newX = coord.X - 1; newX >= 0; newX--)
            {
                if (gameManager.pieces[newX][coord.Y].Equals(GameManager.Piece.None))
                {
                    return;
                }
                else if (gameManager.pieces[newX][coord.Y].Equals(pieceToCheck))
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
        GameManager.Piece pieceToCheck = (gameManager.Turn == GameManager.GameTurn.Player) ? GameManager.Piece.White : GameManager.Piece.Black; 
        
        if (coord.X != 0 && coord.Y != board.Size - 1)
        {
            for (int newX = coord.X - 1, newY = coord.Y + 1; newX >= 0 && newY < board.Size - 1; newX--, newY++)
            {
                if (gameManager.pieces[newX][newY].Equals(GameManager.Piece.None))
                {
                    return;
                }
                else if (gameManager.pieces[newX][newY].Equals(pieceToCheck))
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
    #endregion
}








