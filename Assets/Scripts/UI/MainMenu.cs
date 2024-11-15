using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject tutorial;

    private void Start()
    {
        Time.timeScale = 1f;
    }
    public void Exit()
    {
        Application.Quit();
    }

    public void StartGame(GameObject loadingText)
    {
        loadingText.SetActive(true);
        SceneManager.LoadScene("Game");
    }

    public void CloseTutorialScreen()
    {
        tutorial.SetActive(false);
    }
    public void OpenTutorialScreen()
    {
        tutorial.SetActive(true);
    }
}
