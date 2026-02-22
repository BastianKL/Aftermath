using UnityEngine;
using UnityEngine.Events;

public class FireTree : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem fireParticles;
    public Renderer treeRenderer;
    public Material leavesMaterialAfterFire;
    public Material trunkMaterialAfterFire;
    public UnityEvent onFireExtinguished;

    private bool _isExtinguished = false;
    public bool IsOnFire => !_isExtinguished;

    public void Extinguish()
    {
        if (_isExtinguished) return;

        Debug.Log($"Extinguish called on {gameObject.name}");

        if (fireParticles != null)
        {
            fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            fireParticles.Clear(true);
        }

        if (treeRenderer != null && leavesMaterialAfterFire != null && trunkMaterialAfterFire != null)
        {
            var mats = treeRenderer.materials;
            if (mats.Length >= 2)
            {
                mats[0] = leavesMaterialAfterFire;
                mats[1] = trunkMaterialAfterFire;
                treeRenderer.materials = mats;
            }
        }

        _isExtinguished = true;

        if (onFireExtinguished != null)
            onFireExtinguished.Invoke();
    }
}
