using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class EnemyObjectScript : MonoBehaviour
{
    private GameManagerScript gameManagerScript;
    private BreadthFirstSearchScript breadthFirstSearchScript;
    private PlayerMovementScript playerMovementScript;

    public string enemyPosition;

    private float enemySpeed = 0.5f;

    LinkedListScript<string> enemyPath = new LinkedListScript<string>();

    private bool initiated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        breadthFirstSearchScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<BreadthFirstSearchScript>();
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementScript>();

        enemyPosition = gameObject.name;

        initiated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.mazeCreated == true && !initiated)
        {
            StartCoroutine(findPlayer());
            initiated = true;
        }
    }

    public void MoveEnemy(string point)
    {
        GameObject pointToMoveTo = GameObject.Find(point);
        transform.position = pointToMoveTo.transform.position;
        enemyPosition = point;
    }

    public IEnumerator findPlayer()
    {
        while (true)
        {
            if (gameManagerScript.deathScreenActive || gameManagerScript.winScreenActive)
            {
                yield break;
            }
            enemyPath = breadthFirstSearchScript.BreadthFirstSearch(enemyPosition, playerMovementScript.playerPosition);
            Debug.Log(enemyPath.count);
            if (enemyPath.count > 1)
            {
                MoveEnemy(enemyPath[1]);
            }

            if (enemyPosition == playerMovementScript.playerPosition)
            {
                Debug.Log("Enemy caught the player!");
                gameManagerScript.deathScreen.SetActive(true);
                gameManagerScript.deathScreenActive = true;
            }

            yield return new WaitForSeconds(enemySpeed);
        }
    }
}
