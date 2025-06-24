using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private bool isReceiving = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isReceiving)
            {
                StopReceiving();
            }
            else
            {
                StartReceiving();
            }

        }

        if (isReceiving)
            return; // Block input if receiving

        if (Input.GetKeyDown(KeyCode.T))
        {
            UIController.Instance.ShowHand(UIController.HandName.Transmitting);
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            UIController.Instance.ShowHand(UIController.HandName.Idle);
        }


    }

    public void StartReceiving()
    {
        isReceiving = true;
        UIController.Instance.ShowHand(UIController.HandName.Receiving);
    }

    public void StopReceiving()
    {
        isReceiving = false;
        UIController.Instance.ShowHand(UIController.HandName.Idle);
    }
}
