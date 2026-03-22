using UnityEngine;
using UnityEngine.UI;

// Attacca questo script a ciascuno dei 3 generatori piccoli.
// Il giocatore deve sparargli sopra (con PaintBullet) finché non si sovraccarica.
public class GeneratoreMinore : MonoBehaviour
{
    [Header("Statistiche")]
    [Tooltip("Quanti colpi di vernice servono per sovraccaricare questo generatore")]
    public int colpiNecessari = 10;

    [Header("Stato (debug)")]
    public bool isSovraccaricato = false;
    public bool isAttivo = true; // false = resettato nella fase 2, non colpibile

    [Header("Visual Feedback")]
    [Tooltip("Materiale quando il generatore è normale")]
    public Material materialeNormale;
    [Tooltip("Materiale quando è sovraccaricato")]
    public Material materialeSovraccaricato;
    [Tooltip("Materiale quando è resettato (fase 2, non attivo)")]
    public Material materialeResettato;
    [Tooltip("Effetto particelle da spawnare al sovraccarico")]
    public GameObject vfxSovraccarico;
    [Tooltip("Slider UI mondo 3D sopra il generatore (opzionale)")]
    public Slider barraProgressoWorld;

    private int colpiRicevuti = 0;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        AggiornaMateriale();
        AggiornaBarra();
    }

    // Chiamato da PaintBullet quando colpisce questo oggetto
    public void RiceviColore()
    {
        if (!isAttivo || isSovraccaricato) return;

        colpiRicevuti++;
        AggiornaBarra();

        if (colpiRicevuti >= colpiNecessari)
        {
            Sovraccarica();
        }
    }

    void Sovraccarica()
    {
        isSovraccaricato = true;
        isAttivo = false;

        AggiornaMateriale();

        if (vfxSovraccarico != null)
            Instantiate(vfxSovraccarico, transform.position, Quaternion.identity);

        // Notifica il BossFightManager
        BossFightManager.Instance?.OnGeneratoreMinoreSovraccaricato();
    }

    // Chiamato dal BossFightManager per resettare il generatore nella fase 2
    public void Resetta()
    {
        colpiRicevuti = 0;
        isSovraccaricato = false;
        isAttivo = true;
        AggiornaMateriale();
        AggiornaBarra();
    }

    // Chiamato nella fase 2 per renderlo nuovamente colpibile e farlo tornare allo stato base visivo
    public void AttivaFase2()
    {
        colpiRicevuti = 0;
        isSovraccaricato = false;
        isAttivo = true;
        AggiornaMateriale();
        AggiornaBarra();
    }

    void AggiornaMateriale()
    {
        if (rend == null) return;

        if (isSovraccaricato && materialeSovraccaricato != null)
            rend.material = materialeSovraccaricato;
        else if (!isAttivo && materialeResettato != null)
            rend.material = materialeResettato;
        else if (materialeNormale != null)
            rend.material = materialeNormale;
    }

    void AggiornaBarra()
    {
        if (barraProgressoWorld == null) return;
        barraProgressoWorld.maxValue = colpiNecessari;
        barraProgressoWorld.value = colpiRicevuti;
    }
}
