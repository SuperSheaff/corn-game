using UnityEngine;
using UnityEngine.SceneManagement;

public class BigCrow : MonoBehaviour
{
    public float moveSpeed      = 16f;
    public string playerTag     = "Player";
    public string sceneToLoad   = "MainMenuScene"; // Replace with your scene name

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("BigCrow couldn't find player with tag: " + playerTag);
        }
    }

    void Update()
    {
        if (player == null) return;

        // Move toward player
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
