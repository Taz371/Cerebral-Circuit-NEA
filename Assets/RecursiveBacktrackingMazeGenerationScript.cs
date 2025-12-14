using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

// Generates a maze using recursive backtracking (depth-first search)
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

    // Recursive function to generate the maze using DFS
    IEnumerator GenerateMazeRecursive(string point)
    {
        int[] shuffledDirections = ShuffleArray(directions);
        gameManagerScript.ChangeColorRed(point);
        visitedNodes.AddFirst(point);

        for (int i = 0; i < shuffledDirections.Length; i++)
        {
            string nextPoint = gameManagerScript.RemoveWall(point, shuffledDirections[i], visitedNodes);
            if (nextPoint != "")
            {
                yield return new WaitForSeconds(mazeGenerationSpeed);
                yield return StartCoroutine(GenerateMazeRecursive(nextPoint));
            }
        }

        gameManagerScript.ChangeColorWhite(point);
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
        gameManagerScript.ChangeColorRed("0,0");
    }

    void AddToGraph(string point, string newPoint)
    {
        if (!gameManagerScript.mazeGraph.ContainsKey(point))
        {
            gameManagerScript.mazeGraph.Put(point, new LinkedListScript<string>());
            if (!gameManagerScript.mazeGraph.get(point).Contains(newPoint))
            {
                gameManagerScript.mazeGraph.get(point).AddLast(newPoint);
            }
        }
        else
        {
            if (!gameManagerScript.mazeGraph.get(point).Contains(newPoint))
            {
                gameManagerScript.mazeGraph.get(point).AddLast(newPoint);
            }
        }

        if (!gameManagerScript.mazeGraph.ContainsKey(newPoint))
        {
            gameManagerScript.mazeGraph.Put(newPoint, new LinkedListScript<string>());
            if (!gameManagerScript.mazeGraph.get(newPoint).Contains(point))
            {
                gameManagerScript.mazeGraph.get(newPoint).AddLast(point);
            }
        }
        else
        {
            if (!gameManagerScript.mazeGraph.get(newPoint).Contains(point))
            {
                gameManagerScript.mazeGraph.get(newPoint).AddLast(point);
            }
        }
    }
}
