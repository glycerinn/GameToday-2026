using UnityEngine;

public class PullEnemy : MonoBehaviour, IEnemy
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Grapple")]
    public float pullSpeed = 20f;
    public float pullDuration = 0.4f;

    private Transform player;
    private Rigidbody rb;

    private bool pullingPlayer;
    private float pullTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

        if (pullingPlayer)
        {
            pullTimer -= Time.fixedDeltaTime;

            if (pullTimer <= 0f)
            {
                StopPull();
            }
        }
        else
        {
            MoveTowardPlayer();
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.z = 0f;
        rb.linearVelocity = velocity;
    }

    void MoveTowardPlayer()
    {
        Vector3 direction = player.position - rb.position;
        direction.z = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        direction.Normalize();

        rb.linearVelocity = new Vector3(
            direction.x * moveSpeed,
            direction.y * moveSpeed,
            0f
        );
    }

    public void PullToPlayer()
    {
        if (player == null || rb == null)
            return;

        Vector3 direction = rb.position - player.position;
        direction.z = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        pullingPlayer = true;
        pullTimer = pullDuration;

        // Pull the PLAYER toward this enemy.
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector3(
                direction.x * pullSpeed,
                direction.y * pullSpeed,
                0f
            );
        }
    }

    void StopPull()
    {
        pullingPlayer = false;

        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
        }
    }

    public void Die()
    {
        Debug.Log("PULL ENEMY DIED!");

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.EnemyDied(this);
        }

        Destroy(gameObject);
    }
}