using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class RecursiveBacktrackingMazeGenerationScript : MonoBehaviour
{
    public GameObject square;

    private float mazeGenerationSpeed;

    private string point;
    private GameObject block;

    private SpriteRenderer spriteR;

    private string startingPoint;
    private string newPoint;

    private string currentPoint;

    private int[] directions = { -2, -1, 1, 2 };

    private GameObject childObj;

    public Text levelText;

    private string listToString;

    public GameManagerScript gameManagerScript;

    LinkedListScript<string> visitedNodes = new LinkedListScript<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        mazeGenerationSpeed = gameManagerScript.mazeGenerationSpeed;
        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        startingPoint = UnityEngine.Random.Range(0, (int)gameManagerScript.mazeWidth) + "," + UnityEngine.Random.Range(0, (int)gameManagerScript.mazeHeight);

        yield return StartCoroutine(GenerateMazeRecursive(startingPoint));

        gameManagerScript.mazeCreated = true;
    }

    IEnumerator GenerateMazeRecursive(string point)
    {
        int[] shuffledDirections = ShuffleArray(directions);
        ChangeColorRed(point);
        visitedNodes.AddFirst(point);

        for (int i = 0; i < shuffledDirections.Length; i++)
        {
            string nextPoint = RemoveWall(point, shuffledDirections[i]);
            if (nextPoint != "")
            {
                yield return new WaitForSeconds(mazeGenerationSpeed);
                yield return StartCoroutine(GenerateMazeRecursive(nextPoint));
            }
        }

        ChangeColorWhite(point);
        yield return new WaitForSeconds(mazeGenerationSpeed);
    }

    int[] ShuffleArray(int[] array)
    {
        int[] shuffledArray = (int[])array.Clone();
        for (int i = 0; i < shuffledArray.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(i, shuffledArray.Length);
            int temp = shuffledArray[rnd];
            shuffledArray[rnd] = shuffledArray[i];
            shuffledArray[i] = temp;
        }
        return shuffledArray;
    }

    // Update is called once per frame
    void Update()
    {
        levelText.text = $"Level {GameManagerScript.level + 1}";
        ChangeColorRed("0,0");
    }

    public void ChangeColorRed(string point)
    {
        getFilling(point);
        spriteR.color = Color.red;
    }

    void ChangeColorWhite(string point)
    {
        getFilling(point);
        spriteR.color = Color.white;
    }

    void getFilling(string point)
    {
        block = gameManagerScript.pointToObject.get(point);
        if (block != null)
        {
            GameObject childObj = block.transform.Find("Filling").gameObject;
            spriteR = childObj.GetComponent<SpriteRenderer>();
        }
    }

    string RemoveWall(string point, int wallNo)
    {
        // 1 = Left Wall
        // -1 = Right Wall
        // 2 = Top Wall
        // -2 = Bottom Wall

        string[] coords = point.Split(',');
        int x = int.Parse(coords[0]);
        int y = int.Parse(coords[1]);

        if (wallNo == 1 && x > 0)
        {
            newPoint = (x - 1) + "," + y;

            if (!visitedNodes.Contains(newPoint))
            {
                block = gameManagerScript.pointToObject.get(point);
                GameObject childObj = block.transform.Find("Left Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = gameManagerScript.pointToObject.get(newPoint);
                childObj = adjacentBlock.transform.Find("Right Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                AddToGraph(point, newPoint);

                return newPoint;
            }
            else
            {
                return "";
            }
        }
        else if (wallNo == -1 && x < gameManagerScript.mazeWidth - 1)
        {
            newPoint = (x + 1) + "," + y;

            if (!visitedNodes.Contains(newPoint))
            {
                block = gameManagerScript.pointToObject.get(point);
                GameObject childObj = block.transform.Find("Right Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = gameManagerScript.pointToObject.get(newPoint);
                childObj = adjacentBlock.transform.Find("Left Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                AddToGraph(point, newPoint);

                return newPoint;
            }
            else
            {
                return "";
            }
        }
        else if (wallNo == 2 && y > 0)
        {
            newPoint = x + "," + (y - 1);

            if (!visitedNodes.Contains(newPoint))
            {
                block = gameManagerScript.pointToObject.get(point);
                GameObject childObj = block.transform.Find("Top Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = gameManagerScript.pointToObject.get(newPoint);
                childObj = adjacentBlock.transform.Find("Bottom Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                AddToGraph(point, newPoint);

                return newPoint;
            }
            else
            {
                return "";
            }
        }
        else if (wallNo == -2 && y < gameManagerScript.mazeHeight - 1)
        {
            newPoint = x + "," + (y + 1);

            if (!visitedNodes.Contains(newPoint))
            {
                block = gameManagerScript.pointToObject.get(point);
                GameObject childObj = block.transform.Find("Bottom Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = gameManagerScript.pointToObject.get(newPoint);
                childObj = adjacentBlock.transform.Find("Top Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                AddToGraph(point, newPoint);

                return newPoint;
            }
            else
            {
                return "";
            }
        }
        else
        {
            return "";
        }
    }

    void AddToGraph(string point, string newPoint)
    {
        if (!gameManagerScript.mazeGraph.ContainsKey(point))
        {
            gameManagerScript.mazeGraph.Add(point, new List<string>());
            if (!gameManagerScript.mazeGraph[point].Contains(newPoint))
            {
                gameManagerScript.mazeGraph[point].Add(newPoint);
            }
        }
        else
        {
            if (!gameManagerScript.mazeGraph[point].Contains(newPoint))
            {
                gameManagerScript.mazeGraph[point].Add(newPoint);
            }
        }

        if (!gameManagerScript.mazeGraph.ContainsKey(newPoint))
        {
            gameManagerScript.mazeGraph.Add(newPoint, new List<string>());
            if (!gameManagerScript.mazeGraph[newPoint].Contains(point))
            {
                gameManagerScript.mazeGraph[newPoint].Add(point);
            }
        }
        else
        {
            if (!gameManagerScript.mazeGraph[newPoint].Contains(point))
            {
                gameManagerScript.mazeGraph[newPoint].Add(point);
            }
        }
    }
}
