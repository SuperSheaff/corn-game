using UnityEngine;

public class JumpScareTrigger : MonoBehaviour
{
    public string playerTag = "Player";
    public bool oneTime = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && oneTime) return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;
            GameController.Instance?.PlayJumpScare();

            if (oneTime)
            {
                gameObject.SetActive(false); // Disable this trigger after firing
            }
        }
    }
}
