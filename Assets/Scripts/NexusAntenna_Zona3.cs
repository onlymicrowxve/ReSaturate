using UnityEngine;

public class NexusAntenna_Zona3 : MonoBehaviour
{
    [Header("Statistiche Antenna")]
    public int hp = 5;

    [Header("Zona Controllata")]
    public OggettoColorabile[] edificiDaColorare;

    [Header("Robot da disattivare")]
    public RobotAI[] robots;

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
        foreach (OggettoColorabile obj in edificiDaColorare)
            if (obj != null) obj.Ricolora();

        foreach (RobotAI robot in robots)
            if (robot != null) robot.DisattivaRobot();

        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }
}
