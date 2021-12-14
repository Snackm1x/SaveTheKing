using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField]
    private Health health;
    [SerializeField]
    private Image healthBarImage;

    private void Awake()
    {
        health.ClientOnDamageTaken += UpdateUi;
    }

    private void OnDestroy()
    {
        health.ClientOnDamageTaken -= UpdateUi;
    }

    private void UpdateUi(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }
}
