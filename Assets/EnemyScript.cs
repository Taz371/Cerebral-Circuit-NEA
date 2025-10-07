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

    private int numberOfEnemiesInGame = 0;
    public float enemySpeed = 0.5f;

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
            SpawnEnemy();
            StartCoroutine(findPlayer());
        }
    }

    public void MoveEnemy(string point)
    {
        GameObject pointToMoveTo = GameObject.Find(point);
        newEnemy.transform.position = pointToMoveTo.transform.position;
        enemyPosition = point;
    }

    public void SpawnEnemy()
    {
        enemySpawnPoint = UnityEngine.Random.Range(1, (int)gameManagerScript.mazeWidth) + "," + UnityEngine.Random.Range(1, (int)gameManagerScript.mazeHeight);
        enemyPosition = enemySpawnPoint;
        GameObject targetObj = GameObject.Find(enemySpawnPoint);
        newEnemy = Instantiate(enemyObj, targetObj.transform.position, targetObj.transform.rotation);
        numberOfEnemiesInGame++;
        newEnemy.name = $"Enemy {numberOfEnemiesInGame}";
    }

    public IEnumerator findPlayer()
    {
        while (true)
        {
            pathFindingScript.BreadthFirstSearch(gameManagerScript.mazeGraph, enemyPosition, playerMovementScript.playerPosition);

            if (pathFindingScript.path.Count > 1)
            {
                MoveEnemy(pathFindingScript.path[1]);
            }

            if (enemyPosition == playerMovementScript.playerPosition)
            {
                Debug.Log("Enemy caught the player!");
            }

            yield return new WaitForSeconds(enemySpeed);
        }
    }
}
