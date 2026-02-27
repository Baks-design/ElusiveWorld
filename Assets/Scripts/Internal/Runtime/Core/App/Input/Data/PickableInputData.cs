using UnityEngine;

[CreateAssetMenu(menuName = "Data/PickableInputData", order = 0)]
public class PickableInputData : ScriptableObject
{
    bool pickClicked;
    bool pickHold;
    bool pickReleased;

    public bool PickClicked
    {
        get => pickClicked;
        set => pickClicked = value;
    }
    public bool PickHold
    {
        get => pickHold;
        set => pickHold = value;
    }
    public bool PickReleased
    {
        get => pickReleased;
        set => pickReleased = value;
    }
}