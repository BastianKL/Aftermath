using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DumpsterItemBoxInteractable : MonoBehaviour, Interactable
{
    [Header("Animation")]
    public Animator boxAnimator;
    public string useTrigger = "Use";

    [Header("Drop Path")]
    public Transform dropStart;
    public Transform dropEnd;
    public float dragToStartDuration = 0.25f;
    public float dragToEndDuration = 0.35f;
    public float dragArcHeight = 0.15f;

    [Header("Rules")]
    public List<string> allowedTags;
    public int itemsToRemove = 20;

    [Header("Counter")]
    public GameObject counterObject;
    public Texture[] counterTextures;

    [Header("Events")]
    public UnityEvent onThresholdReached;

    [Header("Timing")]
    public float destroyDelayAfterDrop = 0.1f;

    private int itemsRemoved = 0;
    private Renderer counterRenderer;
    private bool busy;

    void Start()
    {
        if (counterObject != null)
            counterRenderer = counterObject.GetComponent<Renderer>();
    }

    public void Interact()
    {
        if (busy) return;

        var player = FindObjectOfType<PlayerMovement>();
        if (player == null) return;

        var heldObj = player.GetHeldItem();
        if (heldObj == null) return;

        var pickup = heldObj.GetComponentInParent<PickupItem>();
        if (pickup == null) return;

        if (!IsAllowedItem(pickup.gameObject)) return;

        StartCoroutine(RemoveItemSequence(player, pickup.gameObject));
    }

    private bool IsAllowedItem(GameObject obj)
    {
        if (allowedTags == null || allowedTags.Count == 0) return true;
        return allowedTags.Contains(obj.tag);
    }

    private IEnumerator RemoveItemSequence(PlayerMovement player, GameObject itemRoot)
    {
        busy = true;

        if (boxAnimator != null && !string.IsNullOrEmpty(useTrigger))
        {
            boxAnimator.ResetTrigger(useTrigger);
            boxAnimator.SetTrigger(useTrigger);
        }

        var pickup = itemRoot.GetComponentInParent<PickupItem>();
        if (pickup != null)
        {
            player.RemoveHeldItemReference(pickup);
            pickup.SetHeld(false);
        }

        PrepareItemForDrag(itemRoot);

        Vector3 from = itemRoot.transform.position;

        if (dropStart != null)
        {
            yield return StartCoroutine(DragItemBezier(itemRoot.transform, from, dropStart.position, dragToStartDuration, dragArcHeight));
        }

        if (dropEnd != null)
        {
            yield return StartCoroutine(DragItemBezier(itemRoot.transform, itemRoot.transform.position, dropEnd.position, dragToEndDuration, 0f));
        }

        if (destroyDelayAfterDrop > 0f)
            yield return new WaitForSeconds(destroyDelayAfterDrop);

        Destroy(itemRoot);
        itemsRemoved++;

        UpdateCounterTexture(itemsRemoved);

        if (itemsRemoved >= itemsToRemove)
            onThresholdReached?.Invoke();

        busy = false;
    }

    private void PrepareItemForDrag(GameObject itemRoot)
    {
        itemRoot.transform.SetParent(null, true);

        foreach (var c in itemRoot.GetComponentsInChildren<Collider>(true))
            c.enabled = false;

        foreach (var rb in itemRoot.GetComponentsInChildren<Rigidbody>(true))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private IEnumerator DragItemBezier(Transform t, Vector3 from, Vector3 to, float duration, float arcHeight)
    {
        if (t == null) yield break;

        if (duration <= 0f)
        {
            t.position = to;
            yield break;
        }

        float time = 0f;
        Vector3 mid = (from + to) * 0.5f + Vector3.up * arcHeight;

        while (time < duration)
        {
            time += Time.deltaTime;
            float u = Mathf.Clamp01(time / duration);

            Vector3 p1 = Vector3.Lerp(from, mid, u);
            Vector3 p2 = Vector3.Lerp(mid, to, u);
            t.position = Vector3.Lerp(p1, p2, u);

            yield return null;
        }

        t.position = to;
    }

    private void UpdateCounterTexture(int removed)
    {
        if (counterRenderer == null) return;
        if (counterTextures == null || counterTextures.Length == 0) return;

        int idx = Mathf.Clamp(removed - 1, 0, counterTextures.Length - 1);
        counterRenderer.material.mainTexture = counterTextures[idx];
    }
}