using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Statistiche Salute")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Interfaccia (UI)")]
    public Slider healthSlider; // Trascina qui uno Slider rosso dalla UI

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void RiceviDanno(int danno)
    {
        currentHealth -= danno;
        
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Muori();
        }
    }

    void Muori()
    {
        Debug.Log("GAME OVER! Sei diventato tutto grigio...");
        // Qui in futuro metteremo il caricamento della scena di Game Over!
    }
}