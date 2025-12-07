using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void SelectRecursiveAndPlay()
    {
        OptionsData.SelectedAlgorithm = MazeAlgorithm.Recursive;
        SceneManager.LoadSceneAsync(1);
    }

    public void SelectIterativeAndPlay()
    {
        OptionsData.SelectedAlgorithm = MazeAlgorithm.Iterative;
        SceneManager.LoadSceneAsync(1);
    }

    public void SelectPrimsAndPlay()
    {
        OptionsData.SelectedAlgorithm = MazeAlgorithm.Prims;
        SceneManager.LoadSceneAsync(1);
    }

    public void LoadOptions()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}