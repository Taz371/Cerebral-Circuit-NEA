using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;


public class AStarScript : MonoBehaviour
{
    List<(float fValue, string node)> openList = new List<(float, string)>();

    HashTableScript<string, string> cameFrom = new HashTableScript<string, string>();
    LinkedListScript<string> path = new LinkedListScript<string>();

    LinkedListScript<string> walkableNeighbours = new LinkedListScript<string>();

    public GameManagerScript gameManagerScript;
    public PlayerMovementScript playerMovementScript;

    private GameObject block;
    private SpriteRenderer spriteR;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementScript>();
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) == true)
        {
            StartCoroutine(SolveMaze());
        }
    }

    public IEnumerator SolveMaze()
    {
        yield return StartCoroutine(aStar(playerMovementScript.playerPosition, gameManagerScript.winPoint));
        int i = 0;
        while (i < path.count && path[i] != gameManagerScript.winPoint)
        {
            ChangeColorBlue(path[i]);
            i++;
            yield return new WaitForSeconds(0f);
        }
    }

    void ChangeColorRed(string point)
    {
        block = gameManagerScript.pointToObject.get(point);

        if (block != null)
        {
            GameObject childObj = block.transform.Find("Filling").gameObject;

            spriteR = childObj.GetComponent<SpriteRenderer>();
        }

        spriteR.color = Color.red;
    }

    void ChangeColorBlue(string point)
    {
        block = gameManagerScript.pointToObject.get(point);

        if (block != null)
        {
            GameObject childObj = block.transform.Find("Filling").gameObject;

            spriteR = childObj.GetComponent<SpriteRenderer>();
        }

        spriteR.color = Color.blue;
    }

    public IEnumerator aStar(string startPos, string targetPos)
    {
        string[] targetCoords = targetPos.Split(',');

        int targetX = int.Parse(targetCoords[0]);
        int targetY = int.Parse(targetCoords[1]);

        Dictionary<string, float> gScore = new Dictionary<string, float>();
        //HashTableScript<string, float> gScore = new HashTableScript<string, float>();
        gScore[startPos] = 0;

        openList.Add((0, startPos));

        while(openList.Count > 0)
        {
            var minItem = openList.OrderBy(item => item.fValue).First();
            ChangeColorRed(minItem.node);
            yield return new WaitForSeconds(0.1f);
            string[] minItemCoords = minItem.node.Split(',');

            int minItemX = int.Parse(minItemCoords[0]);
            int minItemY = int.Parse(minItemCoords[1]);

            if (minItem.node == targetPos)
            {
                Debug.Log("FOUND!");
                break;
            }
            else
            {
                openList.Remove(minItem);

                walkableNeighbours = gameManagerScript.mazeGraph.get(minItem.node);

                foreach (string neighbour in walkableNeighbours.AsEnumerable())
                {
                    string[] nodeCoords = neighbour.Split(',');

                    int nodeX = int.Parse(nodeCoords[0]);
                    int nodeY = int.Parse(nodeCoords[1]);

                    float tentativeG = gScore[minItem.node] + (float)Math.Sqrt(Math.Pow(nodeX - minItemX, 2) + Math.Pow(nodeY - minItemY, 2));

                    if (!gScore.ContainsKey(neighbour) || tentativeG < gScore[neighbour])
                    {
                        gScore[neighbour] = tentativeG;

                        //float h = (float)Math.Sqrt(Math.Pow(targetX - nodeX, 2) + Math.Pow(targetY - nodeY, 2));
                        float h = Math.Abs(targetX - nodeX) + Math.Abs(targetY - nodeY);

                        float f = tentativeG + h;

                        if (!openList.Any(x => x.node == neighbour))
                        {
                            openList.Add((f, neighbour));
                        }

                        cameFrom.Put(neighbour, minItem.node);
                    }
                }
            }
        }

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
