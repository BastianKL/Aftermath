using UnityEngine;

public class PulsateEffect : MonoBehaviour
{
    public float pulsateSpeed = 2f;
    public float minScale = 1.05f;
    public float maxScale = 1.15f;

    private Vector3 baseScale;
    private bool pulsating = false;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (pulsating)
        {
            float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulsateSpeed) + 1f) / 2f);
            transform.localScale = baseScale * scale;
        }
    }

    public void SetPulsate(bool enable)
    {
        pulsating = enable;
        if (!enable)
            transform.localScale = baseScale;
    }
}
