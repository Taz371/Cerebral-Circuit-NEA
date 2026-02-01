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

    // Delay between showing each step of the solution
    public float solveSpeed; 

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
        // Start BFS maze solve visualization when Enter is pressed
        if (Input.GetKeyDown(KeyCode.Return) == true)
        {
            StartCoroutine(SolveMaze());
        }
    }

    public IEnumerator SolveMaze()
    {
        LinkedListScript<string> solvedPath = new LinkedListScript<string>();

        // First animate the BFS search
        yield return StartCoroutine(BreadthFirstSearchAnimation(playerMovementScript.playerPosition, gameManagerScript.winPoint));

        // Get the final BFS path from start to goal
        solvedPath = BreadthFirstSearch(playerMovementScript.playerPosition, gameManagerScript.winPoint);

        // Animate the path by coloring the cells blue
        int i = 0;
        while (i < solvedPath.count && solvedPath[i] != gameManagerScript.winPoint)
        {
            gameManagerScript.ChangeColorBlue(solvedPath[i]);
            i++;
            yield return new WaitForSeconds(solveSpeed);
        }
    }

    public IEnumerator BreadthFirstSearchAnimation(string currentVertex, string pointToFind)
    {
        HashTableScript<string, LinkedListScript<string>> graph = new HashTableScript<string, LinkedListScript<string>>();
        graph = gameManagerScript.mazeGraph;

        HashTableScript<string, string> cameFrom = new HashTableScript<string, string>();
        LinkedListScript<string> visited = new LinkedListScript<string>();

        string[] queue = new string[Queue.MaxSize];
        int front = 0;
        int rear = -1;

        rear = Queue.enQueue(queue, rear, currentVertex);
        visited.AddFirst(currentVertex);
        cameFrom.Put("Start", null);

        while (!Queue.isEmpty(front, rear) && currentVertex != pointToFind)
        {
            currentVertex = Queue.deQueue(queue, ref front, rear);

            foreach (string vertex in graph.get(currentVertex).AsEnumerable())
            {
                if (!visited.Contains(vertex) && !Queue.Contains(queue, vertex))
                {
                    rear = Queue.enQueue(queue, rear, vertex);
                    visited.AddFirst(vertex);
                    gameManagerScript.ChangeColorRed(vertex);
                    yield return new WaitForSeconds(0.1f);
                    cameFrom.Put(vertex, currentVertex);
                }
            }
        }
    }

    public LinkedListScript<string> BreadthFirstSearch(string currentVertex, string pointToFind)
    {
        HashTableScript<string, LinkedListScript<string>> graph = new HashTableScript<string, LinkedListScript<string>>();
        graph = gameManagerScript.mazeGraph;

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

        HashTableScript<string, string> cameFrom = new HashTableScript<string, string>();
        LinkedListScript<string> path = new LinkedListScript<string>();
        LinkedListScript<string> visited = new LinkedListScript<string>();

        string[] queue = new string[Queue.MaxSize];
        int front = 0;
        int rear = -1;

        rear = Queue.enQueue(queue, rear, currentVertex);
        visited.AddFirst(currentVertex);
        cameFrom.Put("Start", null);

        while (!Queue.isEmpty(front, rear) && currentVertex != pointToFind)
        {
            currentVertex = Queue.deQueue(queue, ref front, rear);

            foreach (string vertex in graph.get(currentVertex).AsEnumerable())
            {
                if (!visited.Contains(vertex))
                {
                    rear = Queue.enQueue(queue, rear, vertex);
                    visited.AddFirst(vertex);
                    cameFrom.Put(vertex, currentVertex);
                }
            }
        }

        // Reconstruct path from goal to start using cameFrom
        string current = pointToFind;

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

        return path;
    }
}

// Custom queue implementation used for BFS
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
