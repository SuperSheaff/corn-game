using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;

    private Interactable currentTarget;

    void Update()
    {
        CheckForInteractable();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.OnInteract();
        }
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            currentTarget = hit.collider.GetComponent<Interactable>();
        }
        else
        {
            currentTarget = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * interactRange);
    }

}
