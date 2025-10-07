using UnityEngine;
using UnityEngine.SceneManagement;

public class FillingScript : MonoBehaviour
{
    public SpriteRenderer spriteR;
    private GameObject playerObj;

    public PlayerMovementScript playerMovementScript;
    public GameManagerScript gameManagerScript;

    private bool created = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObj = GameObject.Find("Player").gameObject;
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();

        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementScript>();

        created = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (spriteR.color.Equals(Color.green) && collision.gameObject == playerObj)
        {
            GameManagerScript.level += 1;
            gameManagerScript.win();
        }
        else
        {
            if (created && collision.gameObject == playerObj)
            {
                string parentObjectName = transform.parent.gameObject.name;
                playerMovementScript.playerPosition = parentObjectName;
                //spriteR.color = Color.blue;
            }
        }
    }
}
