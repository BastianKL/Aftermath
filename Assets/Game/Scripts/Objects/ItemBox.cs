using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemBox : MonoBehaviour
{
    public Animator boxAnimator;
    public GameObject counterObject;
    public Texture[] counterTextures;
    public int itemsToRemove = 20;
    public UnityEvent onThresholdReached;

    public List<string> allowedTags; // Add allowed tags in the Inspector

    private int itemsRemoved = 0;
    private Renderer counterRenderer;

    void Start()
    {
        if (counterObject != null)
            counterRenderer = counterObject.GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<PickupItem>();
        if (item != null && IsAllowedItem(other))
        {
            StartCoroutine(RemoveItemSequence(item.gameObject));
        }
    }

    private bool IsAllowedItem(Collider other)
    {
        // Filter by tag
        return allowedTags.Contains(other.tag);
    }

    private System.Collections.IEnumerator RemoveItemSequence(GameObject item)
    {
        if (boxAnimator != null)
            boxAnimator.SetTrigger("Close");

        yield return new WaitForSeconds(1.0f);

        Destroy(item);
        itemsRemoved++;

        if (counterRenderer != null && counterTextures.Length > 0)
        {
            int textureIndex = Mathf.Clamp(itemsRemoved - 1, 0, counterTextures.Length - 1);
            counterRenderer.material.mainTexture = counterTextures[textureIndex];
        }

        if (itemsRemoved == itemsToRemove)
        {
            if (onThresholdReached != null)
                onThresholdReached.Invoke();
        }
    }
}
