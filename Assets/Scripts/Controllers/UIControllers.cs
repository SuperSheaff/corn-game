using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    public GameObject HandIdle;
    public GameObject HandReceiving;
    public GameObject HandTransmitting;

    public GameObject RadioTutorialText;
    public GameObject WalkTutorialText;
    private EventInstance WalkieOnInstance;
    [SerializeField]
    private EventReference _walkieOnEvent;
    private EventInstance WalkieOffInstance;
    [SerializeField]
    private EventReference _walkieOffEvent;

    public enum HandName { Idle, Receiving, Transmitting }

    //fmod
    private

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
        //fmod
        WalkieOnInstance = RuntimeManager.CreateInstance(_walkieOnEvent);
        WalkieOffInstance = RuntimeManager.CreateInstance(_walkieOffEvent);

    }

    public void ShowHand(HandName hand)
    {
        HandIdle.SetActive(false);
        HandReceiving.SetActive(false);
        HandTransmitting.SetActive(false);

        switch (hand)
        {
            case HandName.Idle:
                HandIdle.SetActive(true);
                WalkieOffInstance.start();
                break;
            case HandName.Receiving:
                HandReceiving.SetActive(true);
                break;
            case HandName.Transmitting:
                HandTransmitting.SetActive(true);
                //fmod
                WalkieOnInstance.start();
                break;
        }
    }

    public void WalkieTutorialText(bool show)
    {
        RadioTutorialText.SetActive(show);
    }

    public void MoveTutorialText(bool show)
    {
        WalkTutorialText.SetActive(show);
    }
}
