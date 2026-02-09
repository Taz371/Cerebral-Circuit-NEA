using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;


public class AStarScript : MonoBehaviour
{
    // List of nodes to be evaluated with their f-values (f = g + h)
    List<(float fValue, string node)> openList = new List<(float, string)>();

    // Tracks the path: which node we came from to reach a given node
    HashTableScript<string, string> cameFrom = new HashTableScript<string, string>();
    LinkedListScript<string> path = new LinkedListScript<string>();

    // Neighbours of the current node that can be walked to
    LinkedListScript<string> walkableNeighbours = new LinkedListScript<string>();

    public GameManagerScript gameManagerScript;
    public PlayerMovementScript playerMovementScript;

    bool isSolving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementScript>();
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Trigger A* maze solver when 'F' key is pressed
        if (Input.GetKeyDown(KeyCode.F) == true && !isSolving)
        {
            StartCoroutine(SolveMaze());
        }
    }

    public IEnumerator SolveMaze()
    {
        isSolving = true;
        // Run A* search and wait for it to complete
        yield return StartCoroutine(aStar(playerMovementScript.playerPosition, gameManagerScript.winPoint));

        // Animate the resulting path by colouring cells blue
        int i = 0;
        while (i < path.count && path[i] != gameManagerScript.winPoint)
        {
            gameManagerScript.ChangeColorBlue(path[i]);
            i++;
            yield return new WaitForSeconds(0);
        }
        isSolving = false;
    }

    public IEnumerator aStar(string startPos, string targetPos)
    {
        string[] targetCoords = targetPos.Split(',');

        int targetX = int.Parse(targetCoords[0]);
        int targetY = int.Parse(targetCoords[1]);

        // gScore: cost from start to each node
        HashTableScript<string, float> gScore = new HashTableScript<string, float>();
        gScore.Put(startPos, 0);

        openList.Add((0, startPos));

        while(openList.Count > 0)
        {
            // Get node with lowest f-value
            var minItem = openList.OrderBy(item => item.fValue).First();

            // Color node magenta to show A* exploration
            gameManagerScript.ChangeColorMagenta(minItem.node);
            yield return new WaitForSeconds(0.1f);
            string[] minItemCoords = minItem.node.Split(',');

            int minItemX = int.Parse(minItemCoords[0]);
            int minItemY = int.Parse(minItemCoords[1]);

            // If we've reached the goal, exit the loop
            if (minItem.node == targetPos)
            {
                break;
            }
            else
            {
                openList.Remove(minItem);

                // Get all walkable neighbours of current node
                walkableNeighbours = gameManagerScript.mazeGraph.get(minItem.node);

                foreach (string neighbour in walkableNeighbours.AsEnumerable())
                {
                    string[] nodeCoords = neighbour.Split(',');

                    int nodeX = int.Parse(nodeCoords[0]);
                    int nodeY = int.Parse(nodeCoords[1]);

                    // Tentative gScore = current gScore + distance to neighbour (Euclidean)
                    float tentativeG = gScore.get(minItem.node) + 1;

                    if (!gScore.ContainsKey(neighbour) || tentativeG < gScore.get(neighbour))
                    {
                        gScore.Put(neighbour, tentativeG);

                        // Below is an euclidean estimate that can be used but not the most efficient for mazes
                        //float h = (float)Math.Sqrt(Math.Pow(targetX - nodeX, 2) + Math.Pow(targetY - nodeY, 2));

                        // Heuristic h: Manhattan distance to target
                        float h = Math.Abs(targetX - nodeX) + Math.Abs(targetY - nodeY);

                        float f = tentativeG + h;

                        // Add neighbour to open list if not already present
                        if (!openList.Any(x => x.node == neighbour))
                        {
                            openList.Add((f, neighbour));
                        }

                        // Track the path
                        cameFrom.Put(neighbour, minItem.node);
                    }
                }
            }
        }

        // Reconstruct path from target to start using cameFrom
        string current = targetPos;

        while (current != null)
        {
            path.AddFirst(current);
            if (cameFrom.ContainsKey(current))
            {
                current = cameFrom.get(current);
            }
            else
            {
                current = null;
            }
        }
    }
}
