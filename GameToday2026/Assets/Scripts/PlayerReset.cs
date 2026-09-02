using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    public Rigidbody playerRb;

    public void ResetPlayerPosition(Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point is null!");
            return;
        }

        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        playerRb.isKinematic = true;

        playerRb.position = spawnPoint.position;

        playerRb.isKinematic = false;

        Debug.Log("Player reset to: " + spawnPoint.position);
    }
}