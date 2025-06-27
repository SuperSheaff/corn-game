using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public enum ScarecrowState
{
    OnGround,
    OnPost,
    Standing,
    Hidden
}

public class ScarecrowController : MonoBehaviour
{
    public static ScarecrowController Instance { get; private set; }

    [Header("Mesh Objects")]
    [SerializeField] private GameObject groundScarecrowMesh;
    [SerializeField] private GameObject postScarecrowMesh;
    [SerializeField] private GameObject standingScarecrowMesh;

    [Header("FMOD")]
    [SerializeField] private EventReference _scarecrowCreakEvent;
    private EventInstance scarecrowCreakInstance;

    [Header("State")]
    [SerializeField] private ScarecrowState currentState = ScarecrowState.OnGround;
    private bool hasEnteredPostState = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateVisuals();
    }

    public void SetState(ScarecrowState newState, bool playSound = true)
    {
        if (newState == ScarecrowState.OnPost && !hasEnteredPostState)
        {
            hasEnteredPostState = true;
            GameController.Instance?.SetupScene2();
            Debug.Log("test");
        }

        currentState = newState;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        groundScarecrowMesh.SetActive(currentState  == ScarecrowState.OnGround);
        postScarecrowMesh.SetActive(currentState == ScarecrowState.OnPost);
        standingScarecrowMesh.SetActive(currentState == ScarecrowState.Standing);
    }

    public ScarecrowState GetState() => currentState;
}
