using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attacca questo script a un GameObject vuoto "BossFightManager" nella scena.
// È il cervello che coordina tutto: fasi, generatori, spawn robot, vittoria.
public class BossFightManager : MonoBehaviour
{
    public static BossFightManager Instance;

    // ─────────────────────────────────────────────
    //  RIFERIMENTI
    // ─────────────────────────────────────────────

    [Header("Generatori")]
    public GeneratoreMinore[] generatoriMinori;  // trascina i 3 generatori piccoli
    public GeneratoreCentrale generatoreCentrale;

    [Header("Spawner Robot")]
    public RobotSpawner spawner;

    [Header("UI")]
    [Tooltip("Pannello con scritto 'Sovraccarica i generatori!' - Fase 1")]
    public GameObject UIFase1;
    [Tooltip("Pannello con scritto 'I generatori si sono resettati!' - inizio Fase 2")]
    public GameObject UIFase2;
    [Tooltip("Testo che mostra la fase attuale")]
    public TextMeshProUGUI testoFase;
    [Tooltip("Testo che mostra quanti generatori hai sovraccaricato su 3")]
    public TextMeshProUGUI testoProgresso;

    [Header("Fase 1 - Spawn Robot")]
    public int robotFase1Totali = 8;
    public int robotFase1MaxContemporanei = 3;
    public float robotFase1Intervallo = 4f;

    [Header("Fase 2 - Spawn Robot")]
    public int robotFase2Totali = 12;
    public int robotFase2MaxContemporanei = 5;
    public float robotFase2Intervallo = 3f;

    [Header("Cutscene Finale")]
    [Tooltip("Trascina qui il GameObject con lo script CutsceneFinale")]
    public CutsceneFinale cutsceneFinale;
    [Tooltip("Secondi di pausa prima che parta la cutscene dopo la vittoria")]
    public float pausaPrimaDellaVittoria = 2f;

    // ─────────────────────────────────────────────
    //  STATO INTERNO
    // ─────────────────────────────────────────────

    public enum FaseBoss { Attesa, Fase1, Transizione, Fase2, Vittoria }
    [Header("Stato (debug)")]
    public FaseBoss faseAttuale = FaseBoss.Attesa;

    private int generatoriSovraccaricatiFase1 = 0;
    private int generatoriSovraccaricatiFase2 = 0;

    // ─────────────────────────────────────────────
    //  INIT
    // ─────────────────────────────────────────────

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Nascondi tutte le UI
        if (UIFase1 != null) UIFase1.SetActive(false);
        if (UIFase2 != null) UIFase2.SetActive(false);

        // Inizia la Fase 1 automaticamente all'avvio della scena
        // (oppure puoi chiamare AvviaFase1() da un trigger)
        AvviaFase1();
    }

    // ─────────────────────────────────────────────
    //  FASE 1
    // ─────────────────────────────────────────────

    public void AvviaFase1()
    {
        faseAttuale = FaseBoss.Fase1;
        generatoriSovraccaricatiFase1 = 0;

        // Attiva tutti i generatori minori
        foreach (GeneratoreMinore g in generatoriMinori)
            if (g != null) g.Resetta();

        // Mostra UI
        if (UIFase1 != null) UIFase1.SetActive(true);
        AggiornaTesto("FASE 1 - Sovraccarica i generatori!");
        AggiornaProgresso(0);

        // Inizia spawn robot
        spawner?.AvviaOndata(robotFase1Totali, robotFase1MaxContemporanei, robotFase1Intervallo);

        Debug.Log("Boss Fight: FASE 1 iniziata");
    }

    // Chiamato da GeneratoreMinore.Sovraccarica()
    public void OnGeneratoreMinoreSovraccaricato()
    {
        if (faseAttuale != FaseBoss.Fase1 && faseAttuale != FaseBoss.Fase2) return;

        if (faseAttuale == FaseBoss.Fase1)
        {
            generatoriSovraccaricatiFase1++;
            AggiornaProgresso(generatoriSovraccaricatiFase1);
            Debug.Log($"Boss Fight: Generatore {generatoriSovraccaricatiFase1}/3 sovraccaricato (Fase 1)");

            if (generatoriSovraccaricatiFase1 >= 3)
                CompletaFase1();
        }
        else if (faseAttuale == FaseBoss.Fase2)
        {
            generatoriSovraccaricatiFase2++;
            AggiornaProgresso(generatoriSovraccaricatiFase2);
            Debug.Log($"Boss Fight: Generatore {generatoriSovraccaricatiFase2}/3 sovraccaricato (Fase 2)");

            if (generatoriSovraccaricatiFase2 >= 3)
                CompletaFase2();
        }
    }

    void CompletaFase1()
    {
        faseAttuale = FaseBoss.Transizione;

        // Riempi il generatore centrale a metà
        generatoreCentrale?.CompletaFase1();

        // Ferma lo spawn
        spawner?.StoppaSpawn();

        // Aspetta un po' prima di avviare la fase 2
        Invoke(nameof(AvviaFase2), 3f);

        Debug.Log("Boss Fight: Fase 1 COMPLETATA → Generatore al 50%");
    }

    // ─────────────────────────────────────────────
    //  FASE 2
    // ─────────────────────────────────────────────

    void AvviaFase2()
    {
        faseAttuale = FaseBoss.Fase2;
        generatoriSovraccaricatiFase2 = 0;

        // Resetta i generatori minori (tornano allo stato iniziale)
        foreach (GeneratoreMinore g in generatoriMinori)
            if (g != null) g.AttivaFase2();

        // Aggiorna UI
        if (UIFase1 != null) UIFase1.SetActive(false);
        if (UIFase2 != null) UIFase2.SetActive(true);

        AggiornaTesto("FASE 2 - I generatori si sono resettati! Ricaricali ancora!");
        AggiornaProgresso(0);

        // Inizia spawn robot più difficile
        spawner?.AvviaOndata(robotFase2Totali, robotFase2MaxContemporanei, robotFase2Intervallo);

        Debug.Log("Boss Fight: FASE 2 iniziata");

        // Nascondi UI Fase 2 dopo qualche secondo
        Invoke(nameof(NascondiUIFase2), 4f);
    }

    void NascondiUIFase2()
    {
        if (UIFase2 != null) UIFase2.SetActive(false);
    }

    void CompletaFase2()
    {
        faseAttuale = FaseBoss.Vittoria;

        // Riempi il generatore centrale al 100%
        generatoreCentrale?.CompletaFase2();

        // Ferma tutto lo spawn
        spawner?.DistruggiTuttiRobot();

        AggiornaTesto("SISTEMA SOVRACCARICATO!");
        Debug.Log("Boss Fight: Fase 2 COMPLETATA → VITTORIA!");

        // Avvia la cutscene dopo una pausa
        Invoke(nameof(AvviaCutscene), pausaPrimaDellaVittoria);
    }

    // ─────────────────────────────────────────────
    //  FINE GIOCO
    // ─────────────────────────────────────────────

    void AvviaCutscene()
    {
        if (cutsceneFinale != null)
            cutsceneFinale.AvviaCutscene();
        else
            Debug.LogWarning("BossFightManager: CutsceneFinale non assegnata!");
    }

    // Chiamato da RobotSpawner quando un'ondata è terminata (tutti morti)
    // Non blocca la boss fight: il giocatore può comunque agire sui generatori
    public void OnOndataCompletata()
    {
        Debug.Log("Boss Fight: Ondata completata (tutti i robot morti)");
        // Potresti aggiungere qui un messaggio tipo "Ondata completata!"
    }

    // ─────────────────────────────────────────────
    //  UTILITÀ UI
    // ─────────────────────────────────────────────

    void AggiornaTesto(string testo)
    {
        if (testoFase != null) testoFase.text = testo;
    }

    void AggiornaProgresso(int n)
    {
        if (testoProgresso != null) testoProgresso.text = $"Generatori: {n} / 3";
    }
}
