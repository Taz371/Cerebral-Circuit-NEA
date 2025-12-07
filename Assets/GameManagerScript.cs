using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

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
    public Text timerMessage;

    public bool winScreenActive = false;

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

    public bool mazeCreated = false;

    public float mazeGenerationSpeed;

    public Dictionary<string, List<string>> mazeGraph = new Dictionary<string, List<string>>();

    public HashTableScript<string, GameObject> pointToObject;

    private static bool isCustom = false;
    private static float customMazeWidth;
    private static float customMazeHeight;

    private GameObject block;
    private SpriteRenderer spriteR;

    void Awake()
    {
        pointToObject = new HashTableScript<string, GameObject>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        winPoint = (mazeWidth - 1) + "," + (mazeHeight - 1);

        switch (OptionsData.SelectedAlgorithm)
        {
            case MazeAlgorithm.Recursive:
                recursive = true;
                iterative = false;
                prims = false;
                break;
            case MazeAlgorithm.Iterative:
                recursive = false;
                iterative = true;
                prims = false;
                break;
            case MazeAlgorithm.Prims:
                recursive = false;
                iterative = false;
                prims = true;
                break;
            default:
                recursive = true;
                iterative = false;
                prims = false;
                break;
        }

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

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
        isCustom = true;

        customMazeWidth = float.Parse(customWidth.text);
        customMazeHeight = float.Parse(customHeight.text);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void getFilling(string point)
    {
        block = pointToObject.get(point);
        if (block != null)
        {
            GameObject childObj = block.transform.Find("Filling").gameObject;
            spriteR = childObj.GetComponent<SpriteRenderer>();
        }
    }

    public void SetWinArea()
    {
        getFilling(winPoint);
        spriteR.color = Color.green;
    }
}
