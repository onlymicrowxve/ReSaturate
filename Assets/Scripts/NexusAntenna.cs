using UnityEngine;

public class NexusAntenna : MonoBehaviour
{
    [Header("Statistiche Antenna")]
    public int hp = 5;

    [Header("Zona Controllata")]
    public OggettoColorabile[] edificiDaColorare;

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

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnAntennaDistrutta();

        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }
}
