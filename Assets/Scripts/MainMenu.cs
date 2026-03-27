using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public GameObject mainMenuPanel;
    public GameObject optionsMenuPanel;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Interno Casa");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Debug1()
    {
        SceneManager.LoadScene("Playground Test");
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void LoadCittà()
    {
        SceneManager.LoadScene("Città");
    }
}