using UnityEngine;

public class NexusAntenna : MonoBehaviour
{
    [Header("Statistiche Antenna")]
    public int hp = 5;

    [Header("Zona Controllata")]
    [Tooltip("Trascina qui gli oggetti con lo script OggettoColorabile")]
    public OggettoColorabile[] edificiDaColorare; // <-- ORA CERCA GLI SCRIPT, NON I GAMEOBJECT!

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
        // Passa in rassegna tutti gli oggetti collegati e dice loro di ricolorarsi
        foreach (OggettoColorabile obj in edificiDaColorare)
        {
            if (obj != null)
            {
                obj.Ricolora(); 
            }
        }

        if (esplosioneColorePrefab != null)
        {
            Instantiate(esplosioneColorePrefab, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f); 
    }
}