using UnityEngine;
using UnityEngine.SceneManagement;
 
public class DeathMenu : MonoBehaviour
{
    public static DeathMenu Instance;
 
    [Header("UI")]
    public GameObject deathMenuPanel;
    public GameObject UI;
 
    void Awake()
    {
        Instance = this;
    }
 
    void Start()
    {
        deathMenuPanel.SetActive(false);
    }
 
    // Chiamato da PlayerHealth quando il player muore
    public void MostraMenuMorte()
    {
        deathMenuPanel.SetActive(true);
        if (UI != null) UI.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
 
    public void Respawn()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
 
    public void TornaAlMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("menu");
    }
}