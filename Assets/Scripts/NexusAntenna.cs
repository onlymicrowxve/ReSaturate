using UnityEngine;

public class NexusAntenna : MonoBehaviour
{
    [Header("Statistiche Antenna")]
    public int hp = 5;

    [Header("Zona Controllata")]
    [Tooltip("Trascina qui gli oggetti con lo script OggettoColorabile")]
    public OggettoColorabile[] edificiDaColorare;

    [Header("Effetti Visivi (Opzionale)")]
    public GameObject esplosioneColorePrefab;

    private bool giaDistrutta = false;

    public void RiceviColore(int danno)
    {
        if (giaDistrutta) return;

        hp -= danno;

        if (hp <= 0)
        {
            giaDistrutta = true;
            EsplosioneDiVita();
        }
    }

    void EsplosioneDiVita()
    {
        // Ricolora tutti gli edifici collegati
        foreach (OggettoColorabile obj in edificiDaColorare)
        {
            if (obj != null)
                obj.Ricolora();
        }

        // Effetto esplosione visiva
        if (esplosioneColorePrefab != null)
            Instantiate(esplosioneColorePrefab, transform.position, Quaternion.identity);

        // --- NOTIFICA IL TUTORIAL MANAGER ---
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnAntennaDistrutta();
        // ------------------------------------

        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }
}
