using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

    private GameObject[][] board;

    public GameObject Tile;

    public int Size { get; private set; }
    public float Offset { get; private set; }

    public int Count { get; set; }

    void Awake()
    {
        Offset = 0.05f;
        Count = 0;
    }

	// Sets board dimensions
	public void SetBoardDimensions(int boardSize)
    {
        Camera.main.GetComponent<CameraManager>().enabled = true;

        Size = boardSize;

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
        board = new GameObject[Size][];

        for (int x = 0; x < Size; x++)
        {
            board[x] = new GameObject[Size];
            
            for (int y = 0; y < Size; y++)
            {
                board[x][y] = Tile;
            }
        }
    }

    // Instantiates the board tiles
    private void InstantiateBoard()
    {
        for (int x = 0; x < Size; x++)
        {
            float xCoord = (x == 0) ? x + .5f : x + .5f + (Offset * x);

            for (int y = 0; y < Size; y++)
            {
                float yCoord = (y == 0) ? y + .5f : y + .5f + (Offset * y);

                Vector3 position = new Vector3(xCoord, yCoord, 0);

                Instantiate(board[x][y], position, Quaternion.identity);
            }
        }
    }
}


