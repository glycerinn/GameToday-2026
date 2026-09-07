using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    [Header("References")]
    public Rigidbody playerRb;
    public MapManager mapManager;

    [Header("Fall Settings")]
    public float fallThreshold = -10f;

    private bool isRespawning;
    private AudioManager audioManager;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (transform.position.y < fallThreshold && !isRespawning)
        {
            Transform spawnPoint = mapManager.GetCurrentSpawnPoint();
            audioManager.playFallSFX();
            ResetPlayerPosition(spawnPoint);
        }
    }

    public void ResetPlayerPosition(Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point is null!");
            return;
        }

        if (playerRb == null)
        {
            Debug.LogWarning("Player Rigidbody is null!");
            return;
        }

        isRespawning = true;
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        playerRb.isKinematic = true;

        playerRb.position = spawnPoint.position;

        playerRb.isKinematic = false;

        isRespawning = false;

        Debug.Log("Player reset to: " + spawnPoint.position);
    }
}