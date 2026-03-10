using UnityEngine;

public class OggettoColorabile : MonoBehaviour
{
    [Header("I veri colori di questo oggetto")]
    [Tooltip("Inserisci qui tutti i materiali colorati, Nello STESSO ORDINE in cui sono sul MeshRenderer")]
    public Material[] materialiColorati;

    public void Ricolora()
    {
        Renderer rend = GetComponent<Renderer>();
        
        if (rend != null && materialiColorati.Length > 0)
        {
            // --- LA CORREZIONE ANTI-FREEZE ---
            // Usiamo 'sharedMaterials' al posto di 'materials'. 
            // Sostituisce i colori istantaneamente senza appesantire la memoria!
            rend.sharedMaterials = materialiColorati;
        }
    }
}