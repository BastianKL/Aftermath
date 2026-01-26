using UnityEngine;
using UnityEngine.Events;

public class ItemBox : MonoBehaviour
{
    public Animator boxAnimator; // Animator for box closing
    public GameObject counterObject; // The object whose texture changes
    public Texture[] counterTextures; // Array of textures for the counter
    public int itemsToRemove = 20;
    public UnityEvent onThresholdReached; // Assign door opening logic here

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
        if (item != null)
        {
            StartCoroutine(RemoveItemSequence(item.gameObject));
        }
    }

    private System.Collections.IEnumerator RemoveItemSequence(GameObject item)
    {
        // Play box closing animation
        if (boxAnimator != null)
            boxAnimator.SetTrigger("Close");

        // Wait for animation to finish (adjust time as needed)
        yield return new WaitForSeconds(1.0f);

        Destroy(item);
        itemsRemoved++;

        // Update counter texture
        if (counterRenderer != null && counterTextures.Length > 0)
        {
            int textureIndex = Mathf.Clamp(itemsRemoved - 1, 0, counterTextures.Length - 1);
            counterRenderer.material.mainTexture = counterTextures[textureIndex];
        }

        // Check if threshold reached
        if (itemsRemoved == itemsToRemove)
        {
            if (onThresholdReached != null)
                onThresholdReached.Invoke();
        }
    }
}