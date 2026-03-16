using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vita")]
    public int hpMax = 5;
    public int hpAttuale;

    [Header("UI (opzionale)")]
    public Slider healthSlider;

    void Start()
    {
        hpAttuale = hpMax;
        if (healthSlider != null)
        {
            healthSlider.maxValue = hpMax;
            healthSlider.value = hpAttuale;
        }
    }

    public void RiceviDanno(int danno)
    {
        hpAttuale -= danno;
        hpAttuale = Mathf.Clamp(hpAttuale, 0, hpMax);

        if (healthSlider != null) healthSlider.value = hpAttuale;

        if (hpAttuale <= 0) Muori();
    }

    void Muori()
    {
        if (DeathMenu.Instance != null)
            DeathMenu.Instance.MostraMenuMorte();
        else
            Debug.LogWarning("DeathMenu non trovato nella scena!");
    }
}
