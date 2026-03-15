using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Portale di Uscita")]
    [Tooltip("Il cubo invisibile (Is Trigger) che diventa il portale dopo la missione")]
    public GameObject muroPortale;

    [Tooltip("UI con scritto 'Premi E per andare alla prossima zona' o simile")]
    public GameObject UIuscita;

    [Tooltip("Nome esatto della scena a cui andare")]
    public string prossimaScena = "NomeScena";

    private bool missioneCompletata = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Il portale parte disattivato
        if (muroPortale != null) muroPortale.SetActive(false);
        if (UIuscita != null) UIuscita.SetActive(false);
    }

    // Chiamato da NexusAntenna quando viene distrutta
    public void OnAntennaDistrutta()
    {
        if (missioneCompletata) return;
        missioneCompletata = true;

        Debug.Log("Tutorial: antenna distrutta, portale attivato!");

        if (muroPortale != null) muroPortale.SetActive(true);
    }

    // Chiamato da PortaleTutorial quando il player entra/esce dal trigger
    public void MostraUIUscita(bool mostra)
    {
        if (UIuscita != null) UIuscita.SetActive(mostra);
    }

    // Chiamato da PortaleTutorial quando il player preme E
    public void VaiProssimaScena()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(prossimaScena);
    }
}
