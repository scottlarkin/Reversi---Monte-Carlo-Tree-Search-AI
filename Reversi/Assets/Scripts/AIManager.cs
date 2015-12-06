using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIManager : MonoBehaviour {
	
    private Board board;
	private GameManager manager;

	private static double Cp = 1/Mathf.Sqrt(2);
	public int numExpansions = 100;
	private double[,] lookupTable = new double[1500,1500];

	private int numLookups = 0;
	private int numNoLookups = 0;

	void Awake()
    {
		manager = GameObject.Find("Managers").GetComponent<GameManager>();

        board = GameObject.Find("Board").GetComponent<Board>();

		for (int i = 1; i < 1500; ++i)
			for (int j = i; j < 1500; ++j)
				lookupTable[i,j] = (Cp * Math.Sqrt((Math.Log((double)i)) / (double)j ));


	}

	public double getRHS(int n, int nj){
		if (n < 1500){
			numLookups ++;
			return lookupTable[n, nj];
		}
		numNoLookups++;
		return (2*Cp * Math.Sqrt((2*(Math.Log((double)n))) / (double)nj ));
		
	}

    // Generates and stores all available moves for whoever's turn it is
    
    // Returns the best possible computer move as a List<Point>, including the piece put down
    public List<Point> ComputerMove()
    {
        List<Point> computerMove = new List<Point>();
        Point bestMove = new Point();

        int mostPoints = 0;

		foreach (KeyValuePair<Point, List<Point>> move in board.state.availableMoves)
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

		board.state.availableMoves.TryGetValue(bestMove, out computerMove);
        // Have to add the move itself to the list of Points
        computerMove.Insert(0, bestMove);

        return computerMove;
    }

	public List<Point> ComputerMoveMTCS()
	{
		//Debug.Log ("computer move");

		List<Point> computerMove = new List<Point> ();
		Point bestMove = new Point ();


		MonteCarloNode rootNode = new MonteCarloNode(manager.board.state, this);

		for (int i = 0; i < numExpansions; i++) {
			MonteCarloNode n = TreePolicy(rootNode);
			n.Backup (Simulate (n));
		}
		//Debug.Log ("finished simulating");
		MonteCarloNode maxNode = null;
		//Debug.Log ("maxnode set");
		double maxVal = double.NegativeInfinity;

		foreach (MonteCarloNode node in rootNode.children) {
			if(node.timesVisited == 0){
				continue;
			}
			if((double)node.score/(double)node.timesVisited > maxVal){
				maxNode = new MonteCarloNode(node);
				maxVal = (double)node.score/(double)node.timesVisited;
			}
		}
	
		bestMove = maxNode.point;

		board.state.availableMoves.TryGetValue(bestMove, out computerMove);
		// Have to add the move itself to the list of Points
		computerMove.Insert(0, bestMove);
		
		return computerMove;
	}


	private MonteCarloNode TreePolicy(MonteCarloNode n)
	{
		MonteCarloNode v = n;

		while(v.board.availableMoves.Count != 0)
		{
			v.AddAvailableMoves(v.board.availableMoves.Keys.ToList());
			if(v.availableMoves.Count != 0)
			{
				return v.Expand();
			}
			else
			{
				v = v.BestChild();
			}
		}
		return v;
	}


	public int Simulate(MonteCarloNode node)
	{
		//Debug.Log ("simulate" );
		BoardState board = new BoardState(node.board);
		UnityEngine.Random.seed = (int)Time.timeSinceLevelLoad;
		board.GenerateAvailableMoves();
		while (board.availableMoves.Count != 0) {
			List<Point> moves = board.availableMoves.Keys.ToList();
			int i = UnityEngine.Random.Range(0,moves.Count - 1);

			//List<Point> p = new List<Point>();W

			/*if(!board.AvailableMove(moves[i], ref p))
			{

			}

			Debug.Log (i + "  " + moves.Count() );*/

			board.ApplyMove(board.PlacePiece(moves[i]));
			board.GenerateAvailableMoves();
		}
		//Debug.Log ("simulated");
		if(board.WhiteCount>board.BlackCount)
			return 1;
		else if(board.WhiteCount>board.BlackCount)
			return-1;
		else
			return 0;

	}
}








