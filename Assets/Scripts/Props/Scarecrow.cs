using UnityEngine;

public class Scarecrow : Interactable
{
    public Transform scarecrowPivot; // the part to rotate
    public Vector3 raisedRotationEuler;  // Set in Inspector (e.g., 0, 0, 0)
    public Vector3 loweredRotationEuler; // Set in Inspector (e.g., 90, 0, 0)
    public float rotateSpeed = 2f;

    private bool isRaised = false;

    void Update()
    {
        Quaternion targetRotation = Quaternion.Euler(isRaised ? raisedRotationEuler : loweredRotationEuler);
        scarecrowPivot.rotation = Quaternion.RotateTowards(
            scarecrowPivot.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime * 100f
        );
    }

    public override void OnInteract()
    {
        isRaised = !isRaised;
    }

    public void Raise() => isRaised = true;
    public void Lower() => isRaised = false;
}
