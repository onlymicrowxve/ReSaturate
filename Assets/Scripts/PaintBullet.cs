using UnityEngine;

public class PaintBullet : MonoBehaviour
{
    public GameObject paintSplatPrefab;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("ArmaDaTerra")) return;

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        GameObject splat = Instantiate(paintSplatPrefab, contact.point + contact.normal * 0.01f, Quaternion.LookRotation(contact.normal));
        Destroy(splat, 10f);

        // --- DANNO ALLE ANTENNE NEXUS (vecchia logica) ---
        NexusAntenna antenna = collision.gameObject.GetComponent<NexusAntenna>();
        if (antenna != null) antenna.RiceviColore(1);

        NexusAntenna_Zona2 antenna2 = collision.gameObject.GetComponent<NexusAntenna_Zona2>();
        if (antenna2 != null) antenna2.RiceviColore(1);

        NexusAntenna_Zona3 antenna3 = collision.gameObject.GetComponent<NexusAntenna_Zona3>();
        if (antenna3 != null) antenna3.RiceviColore(1);

        // --- DANNO AI GENERATORI BOSS FIGHT ---
        GeneratoreMinore generatore = collision.gameObject.GetComponent<GeneratoreMinore>();
        if (generatore != null) generatore.RiceviColore();

        // --- FINE ---

        Destroy(gameObject);
    }
}
