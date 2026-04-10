using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private int _sceneNubmer;
    public void loadscene()
    {
        print("test");
        SceneManager.LoadScene(_sceneNubmer);
    }

    public void HomeScene()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitScene()
    {
        Application.Quit();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}