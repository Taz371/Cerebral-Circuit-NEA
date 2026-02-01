using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

// Generates a maze using a randomized version of Prim's algorithm
public class RandomPrimsGenerationScript : MonoBehaviour
{
    public GameObject square;

    private GameObject block;

    private string startingPoint;
    private string newPoint;

    public Text levelText;

    public GameManagerScript gameManagerScript;

    // Frontier cells (on the edge of the maze) and neighbours for connection
    LinkedListScript<string> frontier= new LinkedListScript<string>();
    LinkedListScript<string> neighbours = new LinkedListScript<string>();
    HashTableScript<string, string> pointInOrOut = new HashTableScript<string, string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        // Choose a random starting cell and mark it as part of the maze
        startingPoint = UnityEngine.Random.Range(0, (int)gameManagerScript.mazeWidth) + "," + UnityEngine.Random.Range(0, (int)gameManagerScript.mazeHeight);

        // Add surrounding walls to the frontier
        Mark(startingPoint);
        gameManagerScript.ChangeColorRed(startingPoint);

        yield return new WaitForSeconds(gameManagerScript.mazeGenerationSpeed);
        gameManagerScript.ChangeColorWhite(startingPoint);

        // While the frontier list has walls(cells) in it
        while (frontier.count != 0)
        {
            // Pick a random cell from the frontier
            string randomFrontier = frontier.GetRandom();
            frontier.RemoveValue(randomFrontier);
            gameManagerScript.ChangeColorWhite(randomFrontier);
            string[] fcoords = randomFrontier.Split(',');

            int fx = int.Parse(fcoords[0]);
            int fy = int.Parse(fcoords[1]);

            // Determine neighbours of this frontier cell that are IN
            Neighbours(fx, fy);

            // If no neighbouring IN cells, skip this cell
            if (neighbours.count == 0)
            {
                frontier.RemoveValue(randomFrontier);
                continue;
            }

            // Pick a random neighbour that is already IN and remove wall
            string randomNeighbour = neighbours.GetRandom();
            string[] ncoords = randomNeighbour.Split(',');

            int nx = int.Parse(ncoords[0]);
            int ny = int.Parse(ncoords[1]);

            int direction = Direction(fx, fy, nx, ny);
            RemoveWall(randomFrontier, direction);

            // Mark this cell as IN and add its walls to the frontier
            Mark(randomFrontier);
            yield return new WaitForSeconds(gameManagerScript.mazeGenerationSpeed);
        }

        // Maze is fully generated when frontier is empty
        gameManagerScript.mazeCreated = true;
    }

    // Adds a cell to the frontier if it's valid
    private void AddFrontier(int x, int y)
    {
        if (x >= 0 && y >= 0 && y < gameManagerScript.mazeHeight && x < gameManagerScript.mazeWidth)
        {
            if (pointInOrOut.ContainsKey(x + "," + y) && (pointInOrOut.get(x + "," + y) == "IN" || pointInOrOut.get(x + "," + y) == "OUT"))
            {
                return;
            }

            if (frontier.Contains(x + "," + y))
            {
                return;
            }

            pointInOrOut.Put(x + "," + y, "OUT");
            frontier.AddFirst(x + "," + y); ;
            gameManagerScript.ChangeColorRed(x + "," + y);
        }
    }

    // Marks a cell as part of the maze and adds its neighbours to the frontier
    private void Mark(string point)
    {
        pointInOrOut.Put(point, "IN");

        string[] coords = point.Split(',');

        int x = int.Parse(coords[0]);
        int y = int.Parse(coords[1]);

        AddFrontier(x - 1, y);
        AddFrontier(x + 1, y);
        AddFrontier(x, y - 1);
        AddFrontier(x, y + 1);
    }

    // Finds neighbouring cells that are already in the maze
    private void Neighbours(int x,int y)
    {
        neighbours.Clear();
        if (x > 0 && pointInOrOut.ContainsKey((x - 1) + "," + y) && pointInOrOut.get((x - 1) + "," + y) == "IN")
        {
            neighbours.AddFirst((x - 1) + "," + y);
        }
        if (x < gameManagerScript.mazeWidth-1 && pointInOrOut.ContainsKey((x + 1) + "," + y) && pointInOrOut.get((x + 1) + "," + y) == "IN")
        {
            neighbours.AddFirst((x + 1) + "," + y);
        }
        if (y > 0 && pointInOrOut.ContainsKey(x + "," + (y - 1)) && pointInOrOut.get((x + "," + (y - 1))) == "IN")
        {
            neighbours.AddFirst(x + "," + (y - 1));
        }
        if (y < gameManagerScript.mazeHeight - 1 && pointInOrOut.ContainsKey(x + "," + (y + 1)) && pointInOrOut.get(x + "," + (y + 1)) == "IN")
        {
            neighbours.AddFirst(x + "," + (y + 1));
        }
    }

    // Determines the direction to remove a wall between two cells
    private int Direction(int fx, int fy, int nx, int ny)
    {
        if (fx < nx)
        {
            return -1;
        }
        if (fx > nx)
        {
            return 1;
        }
        if (fy < ny)
        {
            return -2;
        }
        if (fy > ny)
        {
            return 2;
        }

        throw new InvalidOperationException("Invalid entry");
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

    // Removes the wall between a frontier cell and its neighbour
    void RemoveWall(string point, int wallNo)
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

            gameManagerScript.AddToGraph(point, newPoint);

        }
        else if (wallNo == -1 && x < gameManagerScript.mazeWidth - 1)
        {
            newPoint = (x + 1) + "," + y;

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

            gameManagerScript.AddToGraph(point, newPoint);

        }
        else if (wallNo == 2 && y > 0)
        {
            newPoint = x + "," + (y - 1);

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

            gameManagerScript.AddToGraph(point, newPoint);

        }
        else if (wallNo == -2 && y < gameManagerScript.mazeHeight - 1)
        {
            newPoint = x + "," + (y + 1);

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

            gameManagerScript.AddToGraph(point, newPoint);
        }
    }
}
