using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

// Attacca questo script a un GameObject "CutsceneFinale" nella scena.
// Gestisce: blocco del player → dissolvenza → titoli di coda → menu principale.
public class CutsceneFinale : MonoBehaviour
{
    [Header("Riferimenti Script Player (da bloccare)")]
    public MonoBehaviour scriptMovimento;
    public MonoBehaviour scriptTelecamera;
    public MonoBehaviour scriptRaccogliOggetti;
    public MonoBehaviour scriptArma;

    [Header("UI Cutscene")]
    [Tooltip("Pannello nero che copre tutto lo schermo (Image con alpha 0 all'inizio)")]
    public Image pannelloNero;

    [Tooltip("Testo dei titoli di coda (TextMeshProUGUI)")]
    public TextMeshProUGUI testoTitoli;

    [Header("Timing")]
    [Tooltip("Secondi per la dissolvenza al nero")]
    public float durataDissolvenza = 2f;
    [Tooltip("Secondi di attesa al nero prima dei titoli")]
    public float attesaAlNero = 1f;
    [Tooltip("Secondi per la dissolvenza dei titoli (fade in)")]
    public float durataTitoliFadeIn = 3f;
    [Tooltip("Secondi in cui i titoli restano visibili")]
    public float durataTitoliVisibili = 8f;
    [Tooltip("Secondi per la dissolvenza dei titoli (fade out) prima di tornare al menu")]
    public float durataTitoliFadeOut = 2f;

    [Header("Titoli di Coda")]
    [TextArea(10, 30)]
    public string testoCreditsTitolo = "FINE\n\n" +
        "Un gioco creato da\n[IL TUO NOME]\n\n" +
        "Grazie per aver giocato!\n\n" +
        "Sviluppato con Unity\n\n\n" +
        "~ THE END ~";

    [Header("Destinazione Finale")]
    [Tooltip("Nome della scena del menu principale")]
    public string scenaMenu = "menu";

    void Start()
    {
        // Assicurati che il pannello sia trasparente e il testo invisibile
        if (pannelloNero != null)
            pannelloNero.color = new Color(0, 0, 0, 0);

        if (testoTitoli != null)
        {
            testoTitoli.text = testoCreditsTitolo;
            testoTitoli.color = new Color(1, 1, 1, 0);
            testoTitoli.gameObject.SetActive(false);
        }
    }

    // Chiamato dal BossFightManager
    public void AvviaCutscene()
    {
        StartCoroutine(SequenzaCutscene());
    }

    IEnumerator SequenzaCutscene()
    {
        // 1. Blocca il player
        BloccoPlayer(true);

        // 2. Sblocca il cursore per eventuali UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Dissolvenza al nero
        yield return StartCoroutine(FadePannello(0f, 1f, durataDissolvenza));

        // 4. Attesa nel nero
        yield return new WaitForSeconds(attesaAlNero);

        // 5. Mostra il testo dei titoli di coda
        if (testoTitoli != null)
            testoTitoli.gameObject.SetActive(true);

        // 6. Fade IN del testo
        yield return StartCoroutine(FadeTesto(0f, 1f, durataTitoliFadeIn));

        // 7. Tieni i titoli visibili
        yield return new WaitForSeconds(durataTitoliVisibili);

        // 8. Fade OUT del testo
        yield return StartCoroutine(FadeTesto(1f, 0f, durataTitoliFadeOut));

        // 9. Torna al menu principale
        Time.timeScale = 1f;
        SceneManager.LoadScene(scenaMenu);
    }

    // ─────────────────────────────────────────────
    //  HELPER: FADE
    // ─────────────────────────────────────────────

    IEnumerator FadePannello(float alphaInizio, float alphaFine, float durata)
    {
        if (pannelloNero == null) yield break;

        float elapsed = 0f;
        Color c = pannelloNero.color;

        while (elapsed < durata)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(alphaInizio, alphaFine, elapsed / durata);
            pannelloNero.color = c;
            yield return null;
        }

        c.a = alphaFine;
        pannelloNero.color = c;
    }

    IEnumerator FadeTesto(float alphaInizio, float alphaFine, float durata)
    {
        if (testoTitoli == null) yield break;

        float elapsed = 0f;
        Color c = testoTitoli.color;

        while (elapsed < durata)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(alphaInizio, alphaFine, elapsed / durata);
            testoTitoli.color = c;
            yield return null;
        }

        c.a = alphaFine;
        testoTitoli.color = c;
    }

    // ─────────────────────────────────────────────
    //  HELPER: BLOCCO PLAYER
    // ─────────────────────────────────────────────

    void BloccoPlayer(bool blocca)
    {
        if (scriptMovimento != null) scriptMovimento.enabled = !blocca;
        if (scriptTelecamera != null) scriptTelecamera.enabled = !blocca;
        if (scriptRaccogliOggetti != null) scriptRaccogliOggetti.enabled = !blocca;
        if (scriptArma != null) scriptArma.enabled = !blocca;
    }
}
