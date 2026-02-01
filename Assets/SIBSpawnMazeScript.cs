using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

// Generates a maze using a stack-based depth-first search (iterative backtracking)
public class SIBSpawnMazeScript : MonoBehaviour
{
    public GameObject square;                                     

    private string startingPoint;
    private int direction = 0;

    // Custom stack used for DFS maze generation
    private string[] stack = new string[Stack.MaxSize];
    private int top = -1;

    private int possiblePaths;

    // Movement directions (used for wall removal logic)
    private int[] directions = {-2, -1, 1, 2};
    private bool moved;

    public Text levelText;

    public GameManagerScript gameManagerScript;

    LinkedListScript<string> visitedNodes = new LinkedListScript<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        StartCoroutine(CreateMaze());
    }

    // Creates the maze using depth-first search with backtracking
    IEnumerator CreateMaze()            
    {
        // Choose a random starting cell
        startingPoint = UnityEngine.Random.Range(0, (int)gameManagerScript.mazeWidth) + "," + UnityEngine.Random.Range(0, (int)gameManagerScript.mazeHeight);

        // Push starting point onto stack
        Stack.push(ref top, stack, startingPoint);

        // Mark start as visited and colour it
        gameManagerScript.ChangeColorRed(startingPoint);
        visitedNodes.AddFirst(startingPoint);

        int[] validDirections = new int[] { -2, -1, 1, 2 };
        direction = validDirections[UnityEngine.Random.Range(0, validDirections.Length)];

        // Attempt to remove a wall and move to next cell
        string nextPoint = gameManagerScript.RemoveWall(startingPoint, direction, visitedNodes);

        if (nextPoint != "")
        {
            Stack.push(ref top, stack, nextPoint);
        }

        // Continue until all cells have been visited
        while (!Stack.isEmpty(top))
        {
            moved = false;
            possiblePaths = 4;

            // Get current cell from stack
            nextPoint = Stack.peek(stack, top);
            string[] coords = nextPoint.Split(',');

            int x = int.Parse(coords[0]);
            int y = int.Parse(coords[1]);

            // Count how many neighbouring cells are already visited
            if (x + 1 < gameManagerScript.mazeWidth && visitedNodes.Contains((x + 1) + "," + y))
            {
                possiblePaths -= 1;
            }
            if (x - 1 >= 0 && visitedNodes.Contains((x - 1) + "," + y))
            {
                possiblePaths -= 1;
            }
            if (y + 1 < gameManagerScript.mazeHeight && visitedNodes.Contains(x + "," + (y + 1)))
            {
                possiblePaths -= 1;
            }
            if (y - 1 >= 0 && visitedNodes.Contains(x + "," + (y - 1)))
            {
                possiblePaths -= 1;
            }

            // Backtrack if no valid moves remain
            if (possiblePaths == 0)
            {
                nextPoint = Stack.pop(ref top, stack);
                gameManagerScript.ChangeColorWhite(nextPoint);
                visitedNodes.AddFirst(nextPoint);
                yield return new WaitForSeconds(gameManagerScript.mazeGenerationSpeed);
            }
            else
            {
                // Try directions in random order
                int[] shuffledDirections = ShuffleArray(directions);
                gameManagerScript.ChangeColorRed(nextPoint);
                visitedNodes.AddFirst(nextPoint);

                for (int i = 0; i < shuffledDirections.Length; i++)
                {
                    string currentPoint = gameManagerScript.RemoveWall(nextPoint, shuffledDirections[i], visitedNodes);
                    if (currentPoint != "")
                    {
                        yield return new WaitForSeconds(gameManagerScript.mazeGenerationSpeed);
                        gameManagerScript.ChangeColorRed(currentPoint);
                        visitedNodes.AddFirst(currentPoint);
                        Stack.push(ref top, stack, currentPoint);

                        moved = true;
                        break;
                    }
                }

                // Backtrack if no move was possible
                if (!moved)
                {
                    nextPoint = Stack.pop(ref top, stack);
                    gameManagerScript.ChangeColorWhite(nextPoint);
                    yield return new WaitForSeconds(gameManagerScript.mazeGenerationSpeed);
                }
            }
        }

        gameManagerScript.mazeCreated = true;
    }

    // Randomly shuffles direction array
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
}

// Custom stack implementation used for DFS
internal class Stack
{
    public static int MaxSize = 10000;

    public static bool IsFull(int top)
    {
        if (top == MaxSize - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void push(ref int top, string[] stack, string value)
    {
        if (!IsFull(top))
        {
            top += 1;
            stack[top] = value;
        }
        else
        {
            Debug.Log("Stack is full, data not added");
        }
    }

    public static string pop(ref int top, string[] stack)
    {
        string poppedItem;
        if (isEmpty(top))
        {
            Debug.Log("Stack is empty nothing to pop");
            poppedItem = "";
        }
        else
        {
            poppedItem = stack[top];
            top -= 1;
        }
        return poppedItem;
    }

    public static bool isEmpty(int top)
    {
        if (top == -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string peek(string[] stack, int top)
    {
        string peekedItem;
        if (isEmpty(top))
        {
            Debug.Log("Stack is empty nothing to peek");
            peekedItem = "";
        }
        else
        {
            peekedItem = stack[top];
        }
        return peekedItem;
    }

    public static void printStack(string[] stack, int top)
    {
        if (!isEmpty(top))
        {
            for (int i = 0; i <= top; i++)
            {
                Debug.Log(stack[i]);
            }
        }
        else
        {
            Debug.Log("Stack is empty");
        }
    }
}