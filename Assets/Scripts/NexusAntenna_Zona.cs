using UnityEngine;

public class NexusAntenna_Zona : MonoBehaviour
{
    [Header("Statistiche Antenna")]
    public int hp = 5;

    [Header("Zona Controllata")]
    public OggettoColorabile[] edificiDaColorare;

    [Header("Robot da disattivare (opzionale)")]
    public RobotAI[] robots;

    private bool giaDistrutta = false;

    public void RiceviColore(int danno)
    {
        if (giaDistrutta) return;

        hp -= danno;

        if (hp <= 0)
        {
            giaDistrutta = true;
            Distruggi();
        }
    }

    void Distruggi()
    {
        foreach (OggettoColorabile obj in edificiDaColorare)
            if (obj != null) obj.Ricolora();

        foreach (RobotAI robot in robots)
            if (robot != null) robot.DisattivaRobot();

        if (ZoneManager.Instance != null)
            ZoneManager.Instance.OnAntennaDistrutta();
        else
            Debug.LogWarning("NexusAntenna_Zona: ZoneManager non trovato nella scena!");

        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }
}
