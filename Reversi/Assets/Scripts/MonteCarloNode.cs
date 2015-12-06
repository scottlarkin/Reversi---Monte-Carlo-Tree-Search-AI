using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class MonteCarloNode
{
	public int score;
	public int timesVisited;

	MonteCarloNode parent;
	public List<MonteCarloNode> children;
	public List<MonteCarloNode> availableMoves;
	public Point point;
	public BoardState board;

	AIManager ai;

	public MonteCarloNode(BoardState b, AIManager AI)
	{
		score = 0;
		timesVisited = 0;
		ai = AI;
		parent = null;
		children = new List<MonteCarloNode> ();
		availableMoves = new List<MonteCarloNode> ();
		board = new BoardState(b);
	
		AddAvailableMoves(board.availableMoves.Keys.ToList());
	}

	public MonteCarloNode(MonteCarloNode Parent, Point point)
	{
		score = 0;
		timesVisited = 0;
		ai = Parent.ai;
		parent = Parent;
		children = new List<MonteCarloNode> ();
		availableMoves = new List<MonteCarloNode> (parent.availableMoves);
		this.point = point;
		board = new BoardState(parent.board);
		board.ApplyMove(board.PlacePiece(point));
		board.turn = board.turn == BoardState.GameTurn.Computer ? BoardState.GameTurn.Player : BoardState.GameTurn.Computer;
		board.GenerateAvailableMoves();
		//AddAvailableMoves(board.availableMoves.Keys.ToList());
	}

	public MonteCarloNode(MonteCarloNode n)
	{
		score = n.score;
		timesVisited = n.timesVisited;
		ai = n.ai;
		parent = n.parent;
		children = new List<MonteCarloNode>(n.children);
		availableMoves = new List<MonteCarloNode>(n.availableMoves);
		board = new BoardState(n.board);
		point = n.point;
	}
	
	public void Backup(int val)
	{
		score += val;
		timesVisited++;
		if (parent != null) {
			parent.Backup(val);
		}

	}

	public MonteCarloNode Expand()
	{
		if (availableMoves.Count > 0) {
			MonteCarloNode ret = availableMoves[0];
			AddChild(ret);
			availableMoves.Remove(ret);
			return ret;
		}
		Debug.Log ("really really big problems");
		return null;
	}

	public MonteCarloNode BestChild()
	{
		double bestVal = double.MinValue;
		MonteCarloNode bestChild = null;

		foreach(MonteCarloNode node in children)
		{
			double utc = ((double)node.score / (double)node.timesVisited) + ai.getRHS(timesVisited, node.timesVisited);

			if (utc > bestVal){
				bestChild = node;
				bestVal = utc;
			}
		}
		
		return bestChild;
	}

	public void AddAvailableMoves(List<Point> Points)
	{
			foreach (Point p in Points) {
				availableMoves.Add(new MonteCarloNode (this, p));
			}
	}


	public void AddChildren(List<Point> Points)
	{
		foreach (Point p in Points) {
			AddChild(new MonteCarloNode(this, p));
		}
	}

	public void AddChild(MonteCarloNode Child)
	{
		children.Add(Child);
	}

}

