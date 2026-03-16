using UnityEngine;

// Antenna Scena 2
// Quando distrutta: ricolora edifici + disattiva robot + attiva portale EntrataAzienda
public class NexusAntenna_Zona2 : MonoBehaviour
{
    [Header("Statistiche Antenna")]
    public int hp = 5;

    [Header("Zona Controllata")]
    public OggettoColorabile[] edificiDaColorare;

    [Header("Robot da disattivare")]
    public RobotAI[] robots;

    [Header("Portale")]
    [Tooltip("Trascina qui il cubo invisibile EntrataAzienda")]
    public GameObject entrataAzienda;

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
        // Ricolora edifici
        foreach (OggettoColorabile obj in edificiDaColorare)
            if (obj != null) obj.Ricolora();

        // Disattiva robot
        foreach (RobotAI robot in robots)
            if (robot != null) robot.DisattivaRobot();

        // Attiva portale
        if (entrataAzienda != null) entrataAzienda.SetActive(true);

        // Effetto visivo
        if (esplosioneColorePrefab != null)
            Instantiate(esplosioneColorePrefab, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }
}
