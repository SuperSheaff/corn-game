using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void OnInteract();
    
    public virtual void OnLookAt() { }
    public virtual void OnLookAway() {}
}
