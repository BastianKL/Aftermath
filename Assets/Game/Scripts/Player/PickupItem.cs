using UnityEngine;

public class PickupItem : MonoBehaviour, Interactable
{
    public HandType handType; public GameObject visualEffect;
    private PulsateEffect pulsate;
    public Sprite itemIcon;
    public bool IsHeld { get; private set; }
    public bool unlocksDoubleJump = false;
    public bool unlocksTripleJump = false;

    public bool IsStoredOnBoat { get; set; }
    public BoatCrateRack StoredRack { get; set; }
    public int StoredSlot { get; set; } = -1;

    private PlayerMovement _cachedPlayer;

    void Awake()
    {
        if (visualEffect != null)
            pulsate = visualEffect.GetComponent<PulsateEffect>();

        _cachedPlayer = FindObjectOfType<PlayerMovement>();
    }

    public void Interact()
    {
        if (_cachedPlayer == null) return;

        if (IsStoredOnBoat && StoredRack != null && StoredSlot >= 0)
        {
            StoredRack.Unstore(StoredSlot);
        }

        if (gameObject.CompareTag("Seed"))
        {
            _cachedPlayer.AddSeed();
            Destroy(gameObject);
            return;
        }

        if (unlocksTripleJump)
        {
            _cachedPlayer.UnlockTripleJump();
            Destroy(gameObject);
            return;
        }
        if (unlocksDoubleJump)
        {
            _cachedPlayer.UnlockDoubleJump();
            Destroy(gameObject);
            return;
        }

        switch (handType)
        {
            case HandType.Left:
                _cachedPlayer.HoldItemInHand(this, HandType.Left);
                break;
            case HandType.Right:
                _cachedPlayer.HoldItemInHand(this, HandType.Right);
                break;
            case HandType.Both:
                _cachedPlayer.HoldItemInHand(this, HandType.Both);
                break;
        }
    }

    public void SetHighlight(bool highlight)
    {
        if (visualEffect != null)
            visualEffect.SetActive(highlight);

        if (pulsate != null)
            pulsate.SetPulsate(highlight);
    }

    public void SetHeld(bool held)
    {
        IsHeld = held;
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = held;
        IsHeld = held;
    }
}