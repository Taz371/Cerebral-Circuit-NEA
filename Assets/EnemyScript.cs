using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class EnemyScript : MonoBehaviour
{
    public GameObject enemyObj;
    private GameObject newEnemy;

    private GameManagerScript gameManagerScript;
    private BreadthFirstSearchScript pathFindingScript;
    private PlayerMovementScript playerMovementScript;

    public string enemySpawnPoint;
    public string enemyPosition;

    public int numberOfEnemies;
    private int numberOfEnemiesInGame = 0;
    public float enemySpeed = 0.5f;

    private List<string> enemyPath = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        pathFindingScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<BreadthFirstSearchScript>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManagerScript.mazeCreated == true && numberOfEnemiesInGame == 0)
        {
            SpawnEnemy(numberOfEnemies);
        }
    }

    public void SpawnEnemy(int numberOfEnemies)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            enemySpawnPoint = UnityEngine.Random.Range(1, (int)gameManagerScript.mazeWidth) + "," + UnityEngine.Random.Range(1, (int)gameManagerScript.mazeHeight);
            GameObject targetObj = GameObject.Find(enemySpawnPoint);
            newEnemy = Instantiate(enemyObj, targetObj.transform.position, targetObj.transform.rotation);
            numberOfEnemiesInGame++;
            newEnemy.name = $"{enemySpawnPoint}";
        }
    }
}
