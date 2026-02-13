using UnityEngine;

public class ElevatorButton : MonoBehaviour, Interactable
{
    public ButtonElevator elevator;
    public bool moveUp;

    public void Interact()
    {
        if (elevator == null) return;
        if (moveUp)
            elevator.MoveUp();
        else
            elevator.MoveDown();
    }
}
