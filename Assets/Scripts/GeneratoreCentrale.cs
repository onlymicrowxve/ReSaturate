using UnityEngine;
using UnityEngine.UI;

// Attacca questo script al generatore centrale.
// NON viene colpito direttamente: si riempie automaticamente
// quando i 3 generatori minori vengono sovraccaricati.
public class GeneratoreCentrale : MonoBehaviour
{
    [Header("UI - Barra Centrale")]
    [Tooltip("Slider sulla UI dello schermo (Screen Space) che mostra il progresso")]
    public Slider barraCentrale;

    [Header("Visual Feedback")]
    public Material materialeNormale;
    public Material materialeFase1Completa;   // metà carico
    public Material materialeFase2Completa;   // pieno carico (esplosione finale)
    public GameObject vfxEsplosioneFase1;
    public GameObject vfxEsplosioneFase2;
    [Tooltip("Oggetti che si accendono/animano nella fase 1 (opzionale)")]
    public GameObject[] effettiFase1;
    [Tooltip("Oggetti che si accendono/animano nella fase 2 (opzionale)")]
    public GameObject[] effettiFase2;

    // Lo slider va da 0 a 1.0
    // Fase 1 lo porta a 0.5, Fase 2 lo porta a 1.0
    private float progressoAttuale = 0f;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && materialeNormale != null)
            rend.material = materialeNormale;

        if (barraCentrale != null)
        {
            barraCentrale.minValue = 0f;
            barraCentrale.maxValue = 1f;
            barraCentrale.value = 0f;
        }

        // Effetti inizialmente spenti
        SetEffetti(effettiFase1, false);
        SetEffetti(effettiFase2, false);
    }

    // Chiamato dal BossFightManager al completamento della Fase 1
    public void CompletaFase1()
    {
        progressoAttuale = 0.5f;
        AggiornaBarra();

        if (rend != null && materialeFase1Completa != null)
            rend.material = materialeFase1Completa;

        if (vfxEsplosioneFase1 != null)
            Instantiate(vfxEsplosioneFase1, transform.position, Quaternion.identity);

        SetEffetti(effettiFase1, true);
    }

    // Chiamato dal BossFightManager al completamento della Fase 2
    public void CompletaFase2()
    {
        progressoAttuale = 1f;
        AggiornaBarra();

        if (rend != null && materialeFase2Completa != null)
            rend.material = materialeFase2Completa;

        if (vfxEsplosioneFase2 != null)
            Instantiate(vfxEsplosioneFase2, transform.position, Quaternion.identity);

        SetEffetti(effettiFase2, true);
    }

    void AggiornaBarra()
    {
        if (barraCentrale != null)
            barraCentrale.value = progressoAttuale;
    }

    void SetEffetti(GameObject[] effetti, bool attivi)
    {
        foreach (GameObject g in effetti)
            if (g != null) g.SetActive(attivi);
    }
}
