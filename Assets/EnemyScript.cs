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

    public string enemySpawnPoint;
    public string enemyPosition;

    private int numberOfEnemiesInGame = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManagerScript.mazeCreated == true && numberOfEnemiesInGame == 0)
        {
            SpawnEnemy(GameManagerScript.numberOfEnemies);
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
