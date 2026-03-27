using UnityEngine;
using UnityEngine.SceneManagement;

// Versione generica di TutorialManager, riutilizzabile in qualsiasi scena.
// Mettilo su un GameObject vuoto nella scena (es. "ZoneManager").
public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    [Header("Portale di Uscita")]
    [Tooltip("Il cubo invisibile (Is Trigger) che diventa il portale dopo aver distrutto l'antenna")]
    public GameObject muroPortale;

    [Tooltip("UI con scritto 'Premi E per uscire' o simile")]
    public GameObject UIuscita;

    [Tooltip("Nome esatto della scena a cui andare")]
    public string prossimaScena;

    private bool zonaLiberata = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (muroPortale != null) muroPortale.SetActive(false);
        if (UIuscita != null) UIuscita.SetActive(false);
    }

    // Chiamato da NexusAntenna_Zona (qualsiasi variante) quando viene distrutta
    public void OnAntennaDistrutta()
    {
        if (zonaLiberata) return;
        zonaLiberata = true;

        if (muroPortale != null) muroPortale.SetActive(true);
    }

    // Chiamato da PortaleZona quando il player entra/esce
    public void MostraUIUscita(bool mostra)
    {
        if (UIuscita != null) UIuscita.SetActive(mostra);
    }

    // Chiamato da PortaleZona quando il player preme E
    public void VaiProssimaScena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(prossimaScena);
    }
}
