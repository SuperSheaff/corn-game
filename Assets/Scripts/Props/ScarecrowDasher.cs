using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ScarecrowDasher : MonoBehaviour
{
    public Transform targetPosition;      // Where the scarecrow will dash to
    public float dashSpeed = 20f;         // Speed of dash movement
    public string playerTag = "Player";   // Trigger detection tag

    private bool isDashing = false;

    private EventInstance scarecrowDashSting;
    [SerializeField] private EventReference _scarecrowDashSting;

    private void Start()
    {
        scarecrowDashSting = RuntimeManager.CreateInstance(_scarecrowDashSting);
    }

    void Update()
    {
        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, dashSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition.position) < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDashing && other.CompareTag(playerTag))
        {
            isDashing = true;
            scarecrowDashSting.start();
            GameController.Instance?.SetupScene4();
        }
    }
}
