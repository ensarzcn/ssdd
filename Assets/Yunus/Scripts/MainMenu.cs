using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Neon City");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyun kapandý!");
    }
}
