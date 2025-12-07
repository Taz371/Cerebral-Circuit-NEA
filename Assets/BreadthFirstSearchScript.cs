using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using System.Linq;

public class BreadthFirstSearchScript : MonoBehaviour
{
    public GameManagerScript gameManagerScript;
    public PlayerMovementScript playerMovementScript;
    public EnemyScript enemyScript;

    public float solveSpeed;
    private Coroutine solveRoutine;

    private GameObject block;
    private SpriteRenderer spriteR;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementScript>();
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        enemyScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<EnemyScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) == true)
        {
            StartCoroutine(SolveMaze());
        }
    }

    public IEnumerator SolveMaze()
    {
        List<string> solvedPath = new List<string>();

        yield return StartCoroutine(BreadthFirstSearchAnimation(gameManagerScript.mazeGraph, playerMovementScript.playerPosition, gameManagerScript.winPoint));
        Debug.Log("Drawing Path");

        solvedPath = BreadthFirstSearch(gameManagerScript.mazeGraph, playerMovementScript.playerPosition, gameManagerScript.winPoint);

        int i = 0;
        while (i < solvedPath.Count && solvedPath[i] != gameManagerScript.winPoint)
        {
            ChangeColorBlue(solvedPath[i]);
            i++;
            yield return new WaitForSeconds(solveSpeed);
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

    public IEnumerator BreadthFirstSearchAnimation(Dictionary<string, List<string>> graph, string currentVertex, string pointToFind)
    {
        Dictionary<string, string> cameFrom = new Dictionary<string, string>();
        List<string> path = new List<string>();
        List<string> visited = new List<string>();

        string[] queue = new string[Queue.MaxSize];
        int front = 0;
        int rear = -1;

        rear = Queue.enQueue(queue, rear, currentVertex);
        visited.Add(currentVertex);
        cameFrom.Add("Start", null);

        while (!Queue.isEmpty(front, rear) && currentVertex != pointToFind)
        {
            currentVertex = Queue.deQueue(queue, ref front, rear);

            foreach (string vertex in graph[currentVertex])
            {
                if (!visited.Contains(vertex) && !Queue.Contains(queue, vertex))
                {
                    rear = Queue.enQueue(queue, rear, vertex);
                    visited.Add(vertex);
                    ChangeColorRed(vertex);
                    yield return new WaitForSeconds(0.1f);
                    cameFrom.Add(vertex, currentVertex);
                }
            }
        }
        Debug.Log("FOUND");
    }

    public List<string> BreadthFirstSearch(Dictionary<string, List<string>> graph, string currentVertex, string pointToFind)
    {
        if (graph == null)
        {
            Debug.LogWarning("BFS: graph is null");
            return null;
        }
        if (string.IsNullOrEmpty(currentVertex))
        {
            Debug.LogWarning("BFS: start is null or empty");
            return null;
        }
        if (string.IsNullOrEmpty(pointToFind))
        {
            Debug.LogWarning("BFS: goal is null or empty");
            return null;
        }
        if (!graph.ContainsKey(currentVertex))
        {
            Debug.LogWarning($"BFS: start '{currentVertex}' not found in graph");
            return null;
        }


        Dictionary<string, string> cameFrom = new Dictionary<string, string>();
        List<string> path = new List<string>();
        List<string> visited = new List<string>();

        string[] queue = new string[Queue.MaxSize];
        int front = 0;
        int rear = -1;

        rear = Queue.enQueue(queue, rear, currentVertex);
        visited.Add(currentVertex);
        cameFrom.Add("Start", null);

        while (!Queue.isEmpty(front, rear) && currentVertex != pointToFind)
        {
            currentVertex = Queue.deQueue(queue, ref front, rear);

            foreach (string vertex in graph[currentVertex])
            {
                if (!visited.Contains(vertex) && !Queue.Contains(queue, vertex))
                {
                    rear = Queue.enQueue(queue, rear, vertex);
                    visited.Add(vertex);
                    cameFrom.Add(vertex, currentVertex);
                }
            }
        }

        Debug.Log("FOUND");

        string current = pointToFind;

        while (current != null)
        {
            path.Add(current);
            if (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
            }
            else
            {
                current = null;
            }
        }

        path.Reverse();
        return path;
    }
}

internal class Queue
{
    public const int MaxSize = 300000;

    public static bool isFull(int rear)
    {
        if (rear + 1 == MaxSize)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool isEmpty(int front, int rear)
    {
        if (front > rear)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static int enQueue(string[] queue, int rear, string data)
    {
        if (isFull(rear))
        {
            Debug.Log($"Queue is full - {data} not added");
        }
        else
        {
            rear += 1;
            queue[rear] = data;
        }
        return rear;
    }

    public static string deQueue(string[] queue, ref int front, int rear)
    {
        string deQueuedItem;
        if (isEmpty(front, rear))
        {
            Debug.Log("Queue is empty - nothing to dequeue");
            deQueuedItem = "";
        }
        else
        {
            deQueuedItem = queue[front];
            front += 1;
        }
        return deQueuedItem;
    }

    public static void printQueue(string[] queue, int front, int rear)
    {
        for (int i = front; i <= rear; i++)
        {
            Debug.Log(queue[i]);
        }
    }

    public static bool Contains(string[] queue, string data)
    {
        for (int i = 0; i < queue.Length; i++)
        {
            if (queue[i] == data)
            {
                return true;
            }
        }

        return false;
    }
}
