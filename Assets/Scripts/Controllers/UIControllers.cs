using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    public GameObject HandIdle;
    public GameObject HandReceiving;
    public GameObject HandTransmitting;

    public enum HandName { Idle, Receiving, Transmitting }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
                break;
            case HandName.Receiving:
                HandReceiving.SetActive(true);
                break;
            case HandName.Transmitting:
                HandTransmitting.SetActive(true);
                break;
        }
    }
}
