using UnityEngine;

public class WaterDamageTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private float damageInterval = 0.5f;

    private PlayerHealth playerHealth;
    private bool playerInWater = false;
    private float damageTimer = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = other.GetComponentInParent<PlayerHealth>();
            }
            if (playerHealth == null)
            {
                playerHealth = other.GetComponentInChildren<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                playerInWater = true;
                damageTimer = 0f;
                Debug.Log("Player entered water!");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (playerInWater && playerHealth != null)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                float damage = damagePerSecond * damageInterval;
                playerHealth.TakeDamage(damage);
                damageTimer = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInWater = false;
            playerHealth = null;
            Debug.Log("Player exited water!");
        }
    }
}