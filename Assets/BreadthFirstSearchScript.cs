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
    private Dictionary<string, string> cameFrom = new Dictionary<string, string>();
    public List<string> path = new List<string>();
    private List<string> visited = new List<string>();

    private string[] queue = new string[Queue.MaxSize];
    private int front = 0;
    private int rear = -1;

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
            StartCoroutine(BreadthFirstSearch(gameManagerScript.mazeGraph, playerMovementScript.playerPosition, gameManagerScript.winPoint));
        }
    }

    public IEnumerator SolveMaze()
    {
        BreadthFirstSearch(gameManagerScript.mazeGraph, playerMovementScript.playerPosition, gameManagerScript.winPoint);
        int i = 0;
        while (i < path.Count && path[i] != gameManagerScript.winPoint)
        {
            ChangeColorRed(path[i]);
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

    public IEnumerator BreadthFirstSearch(Dictionary<string, List<string>> graph, string currentVertex, string pointToFind)
    {
        cameFrom.Clear();
        visited.Clear();
        Array.Clear(queue, 0, queue.Length);
        path.Clear();

        front = 0;
        rear = -1;

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
