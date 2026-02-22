using UnityEngine;
using UnityEngine.InputSystem;

public class WateringHose : MonoBehaviour
{
    [Header("Water Settings")]
    public float extinguishTime = 3f;
    public float maxDistance = 10f;
    public ParticleSystem waterParticles;
    public Transform waterParticlesTransform; // Reference to the child transform

    [Header("Input")]
    public InputActionReference useAction; // Assign your LMB action here

    private float fireTimer = 0f;
    private FireTree currentTree = null;
    private bool isUsing = false;

    private void OnEnable()
    {
        if (useAction != null)
        {
            useAction.action.performed += OnUsePerformed;
            useAction.action.canceled += OnUseCanceled;
        }
    }

    private void OnDisable()
    {
        if (useAction != null)
        {
            useAction.action.performed -= OnUsePerformed;
            useAction.action.canceled -= OnUseCanceled;
        }
    }

    private void OnUsePerformed(InputAction.CallbackContext context)
    {
        isUsing = true;
        if (waterParticles != null)
            waterParticles.Play();
    }

    private void OnUseCanceled(InputAction.CallbackContext context)
    {
        isUsing = false;
        if (waterParticles != null)
            waterParticles.Stop();
        fireTimer = 0f;
        currentTree = null;
    }

    private void Update()
    {
        // Rotate the particle child object for dynamic appearance
        if (waterParticlesTransform != null)
        {
            // Rotate around the local forward axis (direction of water spray)
            waterParticlesTransform.Rotate(waterParticlesTransform.forward, 100f * Time.deltaTime, Space.World);
        }

        if (!isUsing) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            FireTree fireTree = hit.collider.GetComponentInParent<FireTree>();
            if (fireTree != null)
            {
                Debug.Log($"Raycast hit {fireTree.gameObject.name}, IsOnFire: {fireTree.IsOnFire}");
                if (fireTree.IsOnFire)
                {
                    if (currentTree != fireTree)
                    {
                        currentTree = fireTree;
                        fireTimer = 0f;
                    }
                    fireTimer += Time.deltaTime;
                    if (fireTimer >= extinguishTime)
                    {
                        Debug.Log($"Extinguishing fire on {fireTree.gameObject.name}");
                        fireTree.Extinguish();
                        currentTree = null;
                        fireTimer = 0f;
                    }
                }
            }
        }
    }
}
