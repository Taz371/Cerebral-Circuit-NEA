using UnityEngine;

public class CreateGridScript : MonoBehaviour
{
    public GameManagerScript gameManagerScript;
    public GameObject square;

    void Start()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManagerScript>();
        CreateGrid();
        AdjustCamera();
    }

    void AdjustCamera()
    {
        GameObject camera = GameObject.Find("Main Camera");
        Camera cameraComponent = camera.GetComponent<Camera>();

        // Formula to position the camera at the centre of the maze
        cameraComponent.orthographicSize = (gameManagerScript.mazeWidth / 2) + 1;
        camera.transform.position = new Vector3((gameManagerScript.mazeWidth / 2) - 0.5f, -1 * ((gameManagerScript.mazeHeight / 2) - 0.5f), -10);
    }

    void CreateGrid()
    {
        for (int xCord = 0; xCord <= gameManagerScript.mazeWidth - 1; xCord++)
        {
            for (int yCord = 0; yCord <= gameManagerScript.mazeHeight - 1; yCord++)
            {
                var newNode = Instantiate(square, new Vector3(xCord, -yCord, 0), transform.rotation);
                newNode.name = xCord + "," + yCord;
                gameManagerScript.pointToObject.Put(newNode.name, newNode);
            }
        }
    }
}
