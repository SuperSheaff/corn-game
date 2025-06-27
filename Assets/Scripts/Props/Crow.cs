using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class Crow : MonoBehaviour
{
    public Animator animator;
    public float scareDistance = 5f;
    public float transmitThreshold = 0.5f;
    public float moveSpeed = 3f;
    public Transform flyTarget; // Optional preset

    public float flyDistance = 10f; // Distance to fly away
    public float flyHeight = 5f;    // Height above current position

    private Transform player;
    private bool isFlying = false;
    private float transmitTimer = 0f;
    private Vector3 destination;

    private float flightDuration;
    private float flightTimer;
    private Vector3 flightStart;
    private Vector3 flatDestination;

    private Vector3 flightNoiseOffset;
    private float noiseFrequency = 5f;
    private float noiseAmplitude = 0.6f;
    private float noiseSeed;

    private bool isDelaying = false;
    private float delayTimer = 0f;
    private float flyDelay = 0f;

    private StudioEventEmitter crowCawInstance;
    //[SerializeField]
    //private EventReference _crowCawEvent;

    private EventInstance flapInstance;
    [SerializeField] private EventReference _flapReference;

    private bool _isCawing;

    private float cawCountdown = 0;

    private MicRecorderUnity micRecorder;

    void Start()
    {
        micRecorder = FindFirstObjectByType<MicRecorderUnity>();
        animator.SetBool("Idle", true); // Idle
        animator.SetBool("Flying", false); // Flying
        animator.SetBool("Cawing", false);
        player = GameObject.FindWithTag("Player").transform;
        //crowCawInstance = RuntimeManager.CreateInstance(_crowCawEvent);
        crowCawInstance = GetComponent<StudioEventEmitter>();
        flapInstance = RuntimeManager.CreateInstance(_flapReference);
    }

    void Update()
    {
        if (isDelaying)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= flyDelay)
            {
                BeginFlight(); // start flying after delay
                isDelaying = false;
            }
            return;
        }

        if (isFlying)
        {
            flightTimer += Time.deltaTime;
            float t = Mathf.Clamp01(flightTimer / flightDuration);

            Vector3 horizontalPos = Vector3.Lerp(flightStart, flatDestination, t);
            float height = Mathf.Sin(t * Mathf.PI) * flyHeight;
            float flutter = Mathf.Sin((flightTimer + noiseSeed) * noiseFrequency) * noiseAmplitude;
            Vector3 noise = flightNoiseOffset * flutter;

            Vector3 newPos = new Vector3(horizontalPos.x, flightStart.y + height, horizontalPos.z) + noise;
            transform.position = newPos;

            if (t >= 1f)
            {
                Destroy(gameObject);
            }

            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= scareDistance && GameController.Instance.IsTransmitting)
        {
            if (micRecorder.IsVoice)
            {
                FlyAway();
            }

			transmitTimer += Time.deltaTime;
            if (transmitTimer >= transmitThreshold) {
                FlyAway();
            } else
			{
				transmitTimer = 0f;
			}
        }

        cawCountdown -= Time.deltaTime;
        if (cawCountdown <= 0)
        {
            Caw();
        }
        //Debug.Log($"{this}{PlaybackState(crowCawInstance.EventInstance)}");
        if (PlaybackState(crowCawInstance.EventInstance) == PLAYBACK_STATE.PLAYING) animator.SetBool("Cawing", false);
    }

    void BeginFlight()
    {
        isFlying = true;
    }

    void FlyAway()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Flying", true);
        Flap();

        // Prep destination and arc
        flightStart = transform.position;
        destination = flyTarget != null ? flyTarget.position : GetRandomFlyPoint();
        flatDestination = new Vector3(destination.x, flightStart.y, destination.z);
        flightDuration = Vector3.Distance(flatDestination, flightStart) / moveSpeed;
        flightTimer = 0f;

        // Random flutter
        noiseSeed = Random.Range(0f, 1000f);
        flightNoiseOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        // Random delay (between 0 and 0.5 seconds based on noise)
        flyDelay = Mathf.PerlinNoise(noiseSeed, 0f) * 0.5f;
        delayTimer = 0f;
        isDelaying = true;
    }

    Vector3 GetRandomFlyPoint()
    {
        Vector3 toCrow = transform.position - player.position;
        Vector3 away = toCrow.normalized;

        Vector3 flatAway = new Vector3(away.x, 0f, away.z).normalized;

        Vector3 flatOffset = flatAway * flyDistance;
        Vector3 verticalOffset = Vector3.up * flyHeight;

        return transform.position + flatOffset + verticalOffset;
    }

    private void Caw()
    {
        //Debug.Log("Caw");
        SendMessage("Play");
        animator.SetBool("Cawing", true);
        cawCountdown = Random.Range(3, 10);
    }

    private void Flap()
    {
        flapInstance.start();
    }
    PLAYBACK_STATE PlaybackState(EventInstance instance)
		{
			PLAYBACK_STATE pS;
			instance.getPlaybackState(out pS);
			return pS;
		}
}
