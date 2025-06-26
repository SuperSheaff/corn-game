using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;

    private Interactable currentTarget = null;
    private Interactable previousTarget = null;

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
            Interactable newTarget = hit.collider.GetComponent<Interactable>();

            if (newTarget != currentTarget)
            {
                if (currentTarget != null)
                    currentTarget.OnLookAway();

                currentTarget = newTarget;

                if (currentTarget != null)
                    currentTarget.OnLookAt();
            }
        }
        else
        {
            if (currentTarget != null)
            {
                currentTarget.OnLookAway();
                currentTarget = null;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * interactRange);
    }

}
