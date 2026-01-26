using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient staminaGradient;

    private void Start()
    {
        if (playerStamina != null)
        {
            playerStamina.OnStaminaChanged.AddListener(UpdateStaminaBar);
            UpdateStaminaBar(playerStamina.GetCurrentStamina(), playerStamina.GetMaxStamina());
        }
    }

    private void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        float fillAmount = currentStamina / maxStamina;
        fillImage.fillAmount = fillAmount;

        if (staminaGradient != null)
        {
            fillImage.color = staminaGradient.Evaluate(fillAmount);
        }
    }

    private void OnDestroy()
    {
        if (playerStamina != null)
        {
            playerStamina.OnStaminaChanged.RemoveListener(UpdateStaminaBar);
        }
    }
}