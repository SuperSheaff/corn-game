using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

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
        if (Input.GetKeyDown(KeyCode.T))
        {
            UIController.Instance.ShowHand(UIController.HandName.Transmitting);
        }

        // On release, clear the hand
        if (Input.GetKeyUp(KeyCode.T))
        {
            UIController.Instance.ShowHand(UIController.HandName.Idle);
        }
    }
}
