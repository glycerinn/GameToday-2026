using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Transform player;

    private float playerZ;

    void Start()
    {
        // Find the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            // Remember the player's Z axis
            playerZ = player.position.z;

            // Force enemy onto the player's Z axis
            Vector3 position = transform.position;
            position.z = playerZ;
            transform.position = position;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        // Move toward player
        Vector3 direction = player.position - transform.position;

        // Don't move on Z
        direction.z = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();

            transform.position +=
                direction * moveSpeed * Time.deltaTime;
        }

        // Keep enemy locked to player's Z
        Vector3 currentPosition = transform.position;
        currentPosition.z = playerZ;
        transform.position = currentPosition;
    }
}