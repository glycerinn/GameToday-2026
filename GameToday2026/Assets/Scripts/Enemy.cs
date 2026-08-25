using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Grapple")]
    public float pullSpeed = 20f;
    public float pullDuration = 0.4f;

    private Transform player;
    private Rigidbody rb;

    private float playerZ;

    private bool beingPulled;
    private float pullTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerZ = player.position.z;

            Vector3 position = rb.position;
            position.z = playerZ;

            rb.position = position;
        }
    }

    void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

        if (beingPulled)
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
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, direction.y * moveSpeed, 0f);
    }

    public void PullToPlayer()
    {
        if (player == null || rb == null)
            return;

        Vector3 direction = player.position - rb.position;
        direction.z = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();
        beingPulled = true;
        pullTimer = pullDuration;

        // Immediately give the enemy strong velocity toward player
        rb.linearVelocity = new Vector3(direction.x * pullSpeed, direction.y * pullSpeed, 0f);
    }

    void StopPull()
    {
        beingPulled = false;
        rb.linearVelocity = Vector3.zero;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}