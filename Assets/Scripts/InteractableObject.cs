using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum Type
    {
        Pickupable,
        Takeable,
        Kickable
    }

    public Type ObjectType = Type.Kickable;
}
