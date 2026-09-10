using UnityEngine;

public class FastEnemy : MonoBehaviour, IEnemy
{
    [Header("Health")]
    public float maxHealth = 50;
    public float enemyhealth { get; set; }

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Grapple")]
    public float pullSpeed = 20f;
    public float pullDuration = 0.4f;

    private Transform player;
    private Rigidbody rb;

    private float playerZ;
    private bool isDead;
    public float health;

    private bool beingPulled;
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
        if (isDead || player == null || rb == null)
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

        rb.linearVelocity = new Vector3(
            direction.x * moveSpeed,
            direction.y * moveSpeed,
            0f
        );
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        enemyhealth -= damage;

        if (enemyhealth <= 0f)
        {
            enemyhealth = 0f;
            Die();
        }
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

        rb.linearVelocity = new Vector3(
            direction.x * pullSpeed,
            direction.y * pullSpeed,
            0f
        );
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (audioManager != null)
            audioManager.playDieSFX();

        // Normal kill
        if (StyleMeter.Instance != null)
        {
            StyleMeter.Instance.EnemyKilled();
        }

        // Check if player is airborne
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerAirState airState =
                playerObject.GetComponent<PlayerAirState>();

            if (airState != null && airState.IsAirborne)
            {
                StyleMeter.Instance.EnemyKilledInAir();

                Debug.Log("AIR KILL! +2 STYLE");
            }
        }

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.EnemyDied(this);
        }

        Destroy(gameObject);
    }

    void StopPull()
    {
        beingPulled = false;
        rb.linearVelocity = Vector3.zero;
    }
    
    public bool OnGrappled()
    {
        // Return false berarti Player yang akan ditarik melesat ke arah musuh ini
        return false;
    }
}