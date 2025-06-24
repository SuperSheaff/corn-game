using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;

public class Scarecrow : Interactable
{
    public Transform scarecrowPivot; // the part to rotate
    public Vector3 raisedRotationEuler;  // Set in Inspector (e.g., 0, 0, 0)
    public Vector3 loweredRotationEuler; // Set in Inspector (e.g., 90, 0, 0)
    public float rotateSpeed = 2f;

    private bool isRaised = false;

    private EventInstance scarecrowCreakInstance;
    [SerializeField]
    private EventReference _scarecrowCreakEvent;

    void Start()
    {
        scarecrowCreakInstance = RuntimeManager.CreateInstance(_scarecrowCreakEvent);   
    }
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
        scarecrowCreakInstance.start();
    }

    public void Raise() => isRaised = true;
    public void Lower() => isRaised = false;
}
