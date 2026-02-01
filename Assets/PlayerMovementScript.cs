using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public float playerSpeed;

    public string playerPosition = "0,0";

    public GameManagerScript gameManagerScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(0, 0, -5);

        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManagerScript.winScreenActive && ! gameManagerScript.deathScreenActive && gameManagerScript.mazeCreated)
        {
            Move();
        }
        else 
        {
            myRigidBody.linearVelocity = Vector3.zero;
        }
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.W) == true || Input.GetKey(KeyCode.UpArrow) == true)
        {
            myRigidBody.linearVelocity = Vector2.up * playerSpeed;
        }
        else if (Input.GetKey(KeyCode.S) == true || Input.GetKey(KeyCode.DownArrow) == true)
        {
            myRigidBody.linearVelocity = Vector2.down * playerSpeed;
        }
        else if (Input.GetKey(KeyCode.A) == true || Input.GetKey(KeyCode.LeftArrow) == true)
        {
            myRigidBody.linearVelocity = Vector2.left * playerSpeed;
        }
        else if (Input.GetKey(KeyCode.D) == true || Input.GetKey(KeyCode.RightArrow) == true)
        {
            myRigidBody.linearVelocity = Vector2.right * playerSpeed;
        }
        else
        {
            myRigidBody.linearVelocity = Vector3.zero;
        }
    }
}
