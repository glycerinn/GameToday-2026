using UnityEngine;

public class PullEnemy : MonoBehaviour, IEnemy
{
    [Header("Health")]
    public float maxHealth = 70f;
    public float enemyhealth { get; set; }

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Grapple")]
    public float pullSpeed = 20f;
    public float pullDuration = 0.4f;

    private Transform player;
    private Rigidbody rb;

    private bool isDead;
    private bool pullingPlayer;
    private float pullTimer;

    private AudioManager audioManager;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        enemyhealth = maxHealth;

        rb = GetComponent<Rigidbody>();

        if (rb == null)
            return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        if (player == null || rb == null)
            return;

        if (pullingPlayer)
        {
            pullTimer -= Time.fixedDeltaTime;

            if (pullTimer <= 0f)
                StopPull();
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

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        enemyhealth -= damage;

        Debug.Log(
            "Pull Enemy Health: " + enemyhealth
        );

        if (enemyhealth <= 0)
            Die();
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

        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector3(direction.x * pullSpeed, direction.y * pullSpeed, 0f);
        }
    }

    void StopPull()
    {
        pullingPlayer = false;

        Rigidbody playerRb =
            player.GetComponent<Rigidbody>();

        if (playerRb != null)
            playerRb.linearVelocity = Vector3.zero;
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (audioManager != null)
            audioManager.playDieSFX();

        if (StyleMeter.Instance != null)
        {
            // Normal kill
            StyleMeter.Instance.EnemyKilled();
        }

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerAirState airState = playerObject.GetComponent<PlayerAirState>();

            if (airState != null && airState.IsAirborne)
            {
                StyleMeter.Instance.EnemyKilledInAir();

                Debug.Log("PULL ENEMY AIR KILL!");
            }
        }

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.EnemyDied(this);
        }

        Destroy(gameObject);
    }
}