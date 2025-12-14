using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

// Manages game logic, timer, maze generation, and win/death conditions
public class GameManagerScript : MonoBehaviour
{
    public Text timerText;
    float timePassed = 0;
    public bool timerPaused = false;

    private float timePassedClone;

    private int seconds;
    private int minutes;

    private float secondsClone;

    private float answer;

    public GameObject winScreen;
    public GameObject deathScreen;
    public Text timerMessage;

    public bool winScreenActive = false;
    public bool deathScreenActive = false;

    public GameObject recursiveMazeSpawner;
    public GameObject iterativeMazeSpawner;
    public GameObject primsMazeSpawner;

    public bool recursive;
    public bool iterative;
    public bool prims;

    public static int level = 0;
    public float mazeWidth;
    public float mazeHeight;
    public string winPoint;

    public Text customWidth;
    public Text customHeight;
    public Text customEnemies;

    public Text typeOfMaze;
    public Slider mazeSpeedSlider;
    public Text mazeSpeedIndicator;

    private string newPoint;

    public bool mazeCreated = false;

    public float mazeGenerationSpeed;

    // Maze graph and mapping of points to GameObjects
    public HashTableScript<string, LinkedListScript<string>> mazeGraph = new HashTableScript<string, LinkedListScript<string>>();
    public HashTableScript<string, GameObject> pointToObject;

    private static bool isCustom = false;
    private static float customMazeWidth;
    private static float customMazeHeight;

    public static int numberOfEnemies = 0;

    private GameObject block;
    private SpriteRenderer spriteR;

    void Awake()
    {
        pointToObject = new HashTableScript<string, GameObject>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Keep slider value after reload
        if (PlayerPrefs.HasKey("MazeSpeed"))
        {
            mazeSpeedSlider.value = PlayerPrefs.GetFloat("MazeSpeed");
        }

        // Set maze size based on level or custom input
        if (!isCustom)
        {
            mazeWidth += level;
            mazeHeight += level;
        }
        else
        {
            mazeWidth = customMazeWidth;
            mazeHeight = customMazeHeight;
        }

        // Bottom-right corner is the winning cell
        winPoint = (mazeWidth - 1) + "," + (mazeHeight - 1);

        // Enable chosen algorithm
        switch (OptionsData.SelectedAlgorithm)
        {
            case MazeAlgorithm.Recursive:
                recursive = true;
                iterative = false;
                prims = false;
                typeOfMaze.text = "Recursive Backtracking";
                break;
            case MazeAlgorithm.Iterative:
                recursive = false;
                iterative = true;
                prims = false;
                typeOfMaze.text = "Iterative Backtracking";
                break;
            case MazeAlgorithm.Prims:
                recursive = false;
                iterative = false;
                prims = true;
                typeOfMaze.text = "Randomised Prim's";
                break;
            default:
                recursive = true;
                iterative = false;
                prims = false;
                typeOfMaze.text = "Recursive Backtracking";
                break;
        }

        // Activate the correct spawner object
        if (recursive)
        {
            recursiveMazeSpawner.SetActive(true);
        }
        if(iterative)
        {
            iterativeMazeSpawner.SetActive(true);
        }
        if (prims)
        {
            primsMazeSpawner.SetActive(true);
        }

        winScreenActive = false;
        deathScreenActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Restart level if Space pressed
        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Timer logic
        if (!timerPaused)
        {
            timePassed += Time.deltaTime;
            minutes = Mathf.FloorToInt(timePassed / 60);
            seconds = Mathf.FloorToInt(timePassed % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (mazeCreated)
        {
            SetWinArea();
        }

        timePassedClone += Time.deltaTime;
        secondsClone = Mathf.FloorToInt(timePassedClone % 60);

        mazeGenerationSpeed = mazeSpeedSlider.value;
        PlayerPrefs.SetFloat("MazeSpeed", mazeSpeedSlider.value);

        // Slider logic
        if (mazeSpeedSlider.value >= 0.33 && mazeSpeedSlider.value <= 0.67)
        {
            mazeSpeedIndicator.text = "Maze Speed: Medium";
        }
        else if (mazeSpeedSlider.value > 0.67)
        {
            mazeSpeedIndicator.text = "Maze Speed: Slow";
        }
        else if (mazeSpeedSlider.value < 0.33 && mazeSpeedSlider.value > 0)
        {
            mazeSpeedIndicator.text = "Maze Speed: Fast";
        }
        else if (mazeSpeedSlider.value == 0)
        {
            mazeSpeedIndicator.text = "Maze Speed: Super Fast";
        }
    }

    public void win()
    {
        winScreenActive = true;
        winScreen.SetActive(true);
        timerPaused = true;
        timerMessage.text = string.Format("Your time was {0:00}:{1:00}", minutes, seconds);
        isCustom = false;
    }

    float[] ShuffleArray(float[] array)
    {
        float[] shuffledArray = (float[])array.Clone();
        for (int i = 0; i < shuffledArray.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(i, shuffledArray.Length);
            float temp = shuffledArray[rnd];
            shuffledArray[rnd] = shuffledArray[i];
            shuffledArray[i] = temp;
        }
        return shuffledArray;
    }

    public void OnEnterButtonClick()
    {
        try
        {
            isCustom = true;

            if ((float.Parse(customWidth.text) == 1 || float.Parse(customHeight.text) == 1) && int.Parse(customEnemies.text) > 0)
            {
                throw new ArgumentException("Please supply at least one argument.");
            }

            customMazeWidth = float.Parse(customWidth.text);
            customMazeHeight = float.Parse(customHeight.text);
            numberOfEnemies = int.Parse(customEnemies.text);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        catch (Exception e)
        {
            Debug.Log("Invalid Input");
        }
    }

    // Change color of a cell
    public void ChangeColorRed(string point)
    {
        GetFilling(point);

        spriteR.color = Color.red;
    }

    public void ChangeColorMagenta(string point)
    {
        GetFilling(point);

        spriteR.color = Color.magenta;
    }


    public void ChangeColorBlue(string point)
    {
        GetFilling(point);

        spriteR.color = Color.blue;
    }

    public void ChangeColorWhite(string point)
    {
        GetFilling(point);

        spriteR.color = Color.white;
    }

    public void SetWinArea()
    {
        GetFilling(winPoint);

        spriteR.color = Color.green;
    }

    void GetFilling(string point)
    {
        block = pointToObject.get(point);

        if (block != null)
        {
            GameObject childObj = block.transform.Find("Filling").gameObject;

            spriteR = childObj.GetComponent<SpriteRenderer>();
        }
    }

    // Add a bidirectional edge between two cells in the maze graph
    public void AddToGraph(string point, string newPoint)
    {
        if (!mazeGraph.ContainsKey(point))
        {
            mazeGraph.Put(point, new LinkedListScript<string>());
            if (!mazeGraph.get(point).Contains(newPoint))
            {
                mazeGraph.get(point).AddLast(newPoint);
            }
        }
        else
        {
            if (!mazeGraph.get(point).Contains(newPoint))
            {
                mazeGraph.get(point).AddLast(newPoint);
            }
        }

        if (!mazeGraph.ContainsKey(newPoint))
        {
            mazeGraph.Put(newPoint, new LinkedListScript<string>());
            if (!mazeGraph.get(newPoint).Contains(point))
            {
                mazeGraph.get(newPoint).AddLast(point);
            }
        }
        else
        {
            if (!mazeGraph.get(newPoint).Contains(point))
            {
                mazeGraph.get(newPoint).AddLast(point);
            }
        }
    }

    // Remove wall function only for the Interative/Recursive Backtracking
    // Removes wall between cells if the new cell hasn't been visited
    public string RemoveWall(string point, int wallNo, LinkedListScript<string> visitedNodes)
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
                block = pointToObject.get(point);
                GameObject childObj = block.transform.Find("Left Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = pointToObject.get(newPoint);
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
        else if (wallNo == -1 && x < mazeWidth - 1)
        {
            newPoint = (x + 1) + "," + y;

            if (!visitedNodes.Contains(newPoint))
            {
                block = pointToObject.get(point);
                GameObject childObj = block.transform.Find("Right Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = pointToObject.get(newPoint);
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
                block = pointToObject.get(point);
                GameObject childObj = block.transform.Find("Top Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = pointToObject.get(newPoint);
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
        else if (wallNo == -2 && y < mazeHeight - 1)
        {
            newPoint = x + "," + (y + 1);

            if (!visitedNodes.Contains(newPoint))
            {
                block = pointToObject.get(point);
                GameObject childObj = block.transform.Find("Bottom Wall").gameObject;
                if (childObj != null)
                {
                    Destroy(childObj);
                }

                GameObject adjacentBlock = pointToObject.get(newPoint);
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
}
