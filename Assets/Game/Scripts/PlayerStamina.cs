using UnityEngine;
using UnityEngine.Events;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaRegenDelay = 1f;

    [Header("Stamina Costs")]
    public float sprintDrainRate = 15f;
    public float spaceBoostDrainRate = 20f;
    public float jumpCost = 20f;
    public float rollCost = 15f;

    [Header("Exhaustion")]
    [SerializeField] private float exhaustionDuration = 3f;
    private bool isExhausted = false;
    private float exhaustionTimer = 0f;

    private float currentStamina;
    private float timeSinceLastUse;
    private bool isRegenerating = true;

    [Header("Events")]
    public UnityEvent<float, float> OnStaminaChanged; // (currentHealth, maxHealth)
    public UnityEvent OnExhausted;
    public UnityEvent OnExhaustionRecovered;

    private void Start()
    {
        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    private void Update()
    {
        // Handle exhaustion - must complete the full duration
        if (isExhausted)
        {
            exhaustionTimer += Time.deltaTime;
            if (exhaustionTimer >= exhaustionDuration)
            {
                isExhausted = false;
                exhaustionTimer = 0f;
                timeSinceLastUse = 0f; // Reset timer for regen delay
                currentStamina = maxStamina * 0.3f; // Recover to 30%
                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
                OnExhaustionRecovered?.Invoke();
                Debug.Log("Recovered from exhaustion! Stamina will regenerate soon.");
            }
            return; // Don't regenerate or count time while exhausted
        }

        // Handle regeneration delay
        if (isRegenerating && currentStamina < maxStamina)
        {
            timeSinceLastUse += Time.deltaTime;

            if (timeSinceLastUse >= staminaRegenDelay)
            {
                RegenerateStamina(staminaRegenRate * Time.deltaTime);
            }
        }
    }

    public bool UseStamina(float amount)
    {
        if (isExhausted) return false;

        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            timeSinceLastUse = 0f; // Reset regen delay timer
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);

            CheckExhaustion();
            return true;
        }
        return false;
    }

    public void DrainStamina(float drainRate)
    {
        if (isExhausted) return;

        currentStamina -= drainRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        timeSinceLastUse = 0f; // Reset regen delay timer
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        CheckExhaustion();
    }

    private void CheckExhaustion()
    {
        if (currentStamina <= 0 && !isExhausted)
        {
            isExhausted = true;
            exhaustionTimer = 0f;
            timeSinceLastUse = 0f;
            OnExhausted?.Invoke();
            Debug.Log($"Player is exhausted! Cannot move for {exhaustionDuration} seconds!");
        }
    }

    public void RegenerateStamina(float amount)
    {
        if (currentStamina < maxStamina && !isExhausted)
        {
            currentStamina += amount;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    public bool HasStamina(float amount)
    {
        return currentStamina >= amount && !isExhausted;
    }

    public bool IsExhausted() => isExhausted;

    public float GetCurrentStamina() => currentStamina;
    public float GetMaxStamina() => maxStamina;

    public void SetRegenerating(bool regen)
    {
        isRegenerating = regen;
    }
}