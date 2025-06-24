using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    private bool isReceiving = false;

    public bool IsTransmitting { get; private set; }

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
        if (!isReceiving)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                IsTransmitting = true;
                UIController.Instance.ShowHand(UIController.HandName.Transmitting);
            }

            if (Input.GetKeyUp(KeyCode.T))
            {
                IsTransmitting = false;
                UIController.Instance.ShowHand(UIController.HandName.Idle);
            }
        }
        else
        {
            IsTransmitting = false;
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
