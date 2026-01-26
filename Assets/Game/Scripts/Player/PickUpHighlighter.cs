using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupHighlighter : MonoBehaviour
{
    public float maxDistance = 3f; // How far you can reach
    private PickupItem lastHighlighted;
    public TextMeshProUGUI pickupHintText;

    public static PickupItem CurrentHighlightedItem { get; private set; }


    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        PickupItem hitItem = null;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            hitItem = hit.collider.GetComponent<PickupItem>();
        }

        if (lastHighlighted != hitItem)
        {
            if (lastHighlighted != null)
                lastHighlighted.SetHighlight(false);
            if (hitItem != null)
                hitItem.SetHighlight(true);

            lastHighlighted = hitItem;
        }
        if (hitItem != null)
        {
            pickupHintText.gameObject.SetActive(true);
            pickupHintText.text = "/ Pick up Item";
        }
        else
        {
            pickupHintText.gameObject.SetActive(false);
        }
    }
}
