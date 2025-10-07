using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class RecursiveBacktrackingMazeGenerationScript : MonoBehaviour
{
    public GameObject square;

    public float mazeGenerationSpeed;
    public float solveMazeGenerationSpeed;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        StartCoroutine(CreateMaze());
        AdjustCamera();
    }

    void SetWinArea()
    {
        getFilling(gameManagerScript.winPoint);
        spriteR.color = Color.green;
    }

    void AdjustCamera()
    {
        GameObject camera = GameObject.Find("Main Camera");
        Camera cameraComponent = camera.GetComponent<Camera>();

        // Formula to position the camera at the centre of the maze
        cameraComponent.orthographicSize = (gameManagerScript.mazeWidth / 2) + 1;
        camera.transform.position = new Vector3((gameManagerScript.mazeWidth / 2) - 0.5f, -1 * ((gameManagerScript.mazeHeight / 2) - 0.5f), -10);
    }

    IEnumerator CreateMaze()
    {
        for (int xCord = 0; xCord <= gameManagerScript.mazeWidth - 1; xCord++)
        {
            for (int yCord = 0; yCord <= gameManagerScript.mazeHeight - 1; yCord++)
            {
                var newNode = Instantiate(square, new Vector3(xCord, -yCord, 0), transform.rotation);
                newNode.name = xCord + "," + yCord;
            }
        }

        startingPoint = UnityEngine.Random.Range(0, (int)gameManagerScript.mazeWidth) + "," + UnityEngine.Random.Range(0, (int)gameManagerScript.mazeHeight);

        yield return StartCoroutine(GenerateMazeRecursive(startingPoint));

        SetWinArea();
        gameManagerScript.mazeCreated = true;
    }

    IEnumerator GenerateMazeRecursive(string point)
    {
        int[] shuffledDirections = ShuffleArray(directions);
        ChangeColorRed(point);
        ChangeLayerToVisited(point);

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
        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

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

    void ChangeLayerToVisited(string point)
    {
        block = GameObject.Find(point);
        block.layer = 3;
    }

    bool isVisited(string point)
    {
        if (point != "")
        {
            block = GameObject.Find(point);
            if (block != null && block.layer == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void getFilling(string point)
    {
        block = GameObject.Find(point);
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

            if (isVisited(newPoint) == false)
            {
                block = GameObject.Find(point);
                GameObject childObj = block.transform.Find("Left Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = GameObject.Find(newPoint);
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

            if (isVisited(newPoint) == false)
            {
                block = GameObject.Find(point);
                GameObject childObj = block.transform.Find("Right Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = GameObject.Find(newPoint);
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

            if (isVisited(newPoint) == false)
            {
                block = GameObject.Find(point);
                GameObject childObj = block.transform.Find("Top Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = GameObject.Find(newPoint);
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

            if (isVisited(newPoint) == false)
            {
                block = GameObject.Find(point);
                GameObject childObj = block.transform.Find("Bottom Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = GameObject.Find(newPoint);
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
