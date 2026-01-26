using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Health Regeneration")]
    [SerializeField] private bool enableRegen = false;
    [SerializeField] private float regenRate = 5f; // Health per second
    [SerializeField] private float regenDelay = 3f; // Seconds after damage before regen starts
    private float timeSinceLastDamage = 0f;

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged; // (currentHealth, maxHealth)
    public UnityEvent OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"PlayerHealth initialized: {currentHealth}/{maxHealth}");
    }

    // TEMPORARY TEST - Press T to test damage
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            TakeDamage(10);
            Debug.Log("Test damage applied!");
        }

        // Handle regeneration
        if (enableRegen && currentHealth < maxHealth && currentHealth > 0)
        {
            timeSinceLastDamage += Time.deltaTime;

            if (timeSinceLastDamage >= regenDelay)
            {
                RegenerateHealth(regenRate * Time.deltaTime);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"TakeDamage called! Damage: {damage}, Current Health: {currentHealth}");
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Reset regeneration timer
        timeSinceLastDamage = 0f;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"New Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void RegenerateHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log("Player died!");
        // Add death logic here (respawn, game over, etc.)
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}