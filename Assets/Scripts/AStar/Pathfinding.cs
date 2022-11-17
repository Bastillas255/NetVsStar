using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
	PathRequestManager requestManager;
	StarGrid grid;

	void Awake()
	{
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<StarGrid>();
	}


	public void StartFindPath(Vector3 startPos,Vector3 targetPos)
    {
		StartCoroutine(FindPath(startPos, targetPos));
    }

	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
	{
		//Stopwatch sw = new Stopwatch();
		//sw.Start();

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		StarNode startNode = grid.NodeFromWorldPoint(startPos);
		StarNode targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
			Heap<StarNode> openSet = new Heap<StarNode>(grid.MaxSize);
			HashSet<StarNode> closedSet = new HashSet<StarNode>();
			openSet.Add(startNode);

			while (openSet.Count > 0)
			{
				StarNode node = openSet.RemoveFirst();
				closedSet.Add(node);

				if (node == targetNode)
				{
					//sw.Stop();
					//print("Path found; " + sw.ElapsedMilliseconds + " ms");
					pathSuccess = true;

					break;
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
		
		yield return null;
        if (pathSuccess)
        {
			waypoints= RetracePath(startNode, targetNode);
		}
		requestManager.FinishedProcessingPath(waypoints, pathSuccess);
	}

	Vector3[] RetracePath(StarNode startNode, StarNode endNode)
	{
		List<StarNode> path = new List<StarNode>();
		StarNode currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		path.Reverse(); //this and next line shouldn't ve here
		grid.path = path;
		return waypoints;

	}

	Vector3[] SimplifyPath(List<StarNode> path)
    {
		List<Vector3> waypoints = new List<Vector3>();
		//Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++)
		{
			//Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX,path[i - 1].gridY - path[i].gridY);
			//if (directionNew != directionOld)
			//{
				waypoints.Add(path[i].worldPosition);
            //}
			//directionOld = directionNew;
        }
		return waypoints.ToArray();
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
