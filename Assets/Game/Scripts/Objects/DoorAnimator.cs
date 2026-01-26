using UnityEngine;

public class DoorAnimator : MonoBehaviour, Interactable
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        animator.SetTrigger("Open");
    }
}
