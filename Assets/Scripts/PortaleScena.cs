using UnityEngine;
using UnityEngine.SceneManagement;

public class PortaleScena : MonoBehaviour
{
    [Header("Scena di destinazione")]
    public string nomeScena;

    [Header("UI (opzionale)")]
    [Tooltip("Testo tipo 'Premi E per entrare' — appare solo quando il player guarda l'oggetto")]
    public GameObject UIinterazione;

    private bool playerVicino = false;

    void Start()
    {
        if (UIinterazione != null) UIinterazione.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerVicino = true;
            if (UIinterazione != null) UIinterazione.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerVicino = false;
            if (UIinterazione != null) UIinterazione.SetActive(false);
        }
    }

    void Update()
    {
        if (playerVicino && Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nomeScena);
        }
    }
}
