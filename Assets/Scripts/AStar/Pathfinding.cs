using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{

	public Transform seeker, target;
	StarGrid grid;

	void Awake()
	{
		grid = GetComponent<StarGrid>();
	}

	void Update()
	{
        if (Input.GetButtonDown("Jump"))
			FindPath(seeker.position, target.position);
	}

	void FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();
		StarNode startNode = grid.NodeFromWorldPoint(startPos);
		StarNode targetNode = grid.NodeFromWorldPoint(targetPos);

		Heap<StarNode> openSet = new Heap<StarNode>(grid.MaxSize);
		HashSet<StarNode> closedSet = new HashSet<StarNode>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			StarNode node = openSet.RemoveFirst();
			closedSet.Add(node);

			if (node == targetNode)
			{
				sw.Stop();
				print("Path found; " + sw.ElapsedMilliseconds + " ms");
				RetracePath(startNode, targetNode);
				return;
			}

			foreach (StarNode neighbour in grid.GetNeighbours(node))
			{
				if (!neighbour.walkable || closedSet.Contains(neighbour))
				{
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}

	void RetracePath(StarNode startNode, StarNode endNode)
	{
		List<StarNode> path = new List<StarNode>();
		StarNode currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		grid.path = path;

	}

	int GetDistance(StarNode nodeA, StarNode nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}
}
