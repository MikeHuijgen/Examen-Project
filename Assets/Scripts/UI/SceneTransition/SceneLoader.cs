using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private int sceneNumber;
    public void LoadScene() => SceneManager.LoadScene(sceneNumber);
    public void HomeScene() => SceneManager.LoadScene(0);
    public void QuitScene() => Application.Quit();
    public void ReloadLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}