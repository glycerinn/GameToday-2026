using System.Collections;
using UnityEngine;

public class ShootEnemy : MonoBehaviour, IEnemy
{
    [Header("Health")]
    public float maxHealth = 30f;
    public float enemyhealth { get; set; }

    [Header("Attack")]
    public Transform firePoint;
    public GameObject enemyBulletPrefab;
    public float shootDelay = 1f;
    public float bulletSpeed = 15f;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Grapple")]
    public float pullSpeed = 20f;
    public float pullDuration = 0.4f;

    private Transform player;
    private Rigidbody rb;
    private float playerZ;

    private bool isDead;
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

        StartCoroutine(Shoot());
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        if (player == null || rb == null)
            return;

        if (beingPulled)
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

    void StopPull()
    {
        beingPulled = false;
        rb.linearVelocity = Vector3.zero;
    }

    IEnumerator Shoot()
    {
        audioManager.playEnemyShootSFX();
        while (!isDead)
        {
            if (PlayerHealth.GameOver)
                yield break;

            yield return new WaitForSeconds(shootDelay);

            if (!isDead && player != null)
                FireBullet();
        }
    }

    void FireBullet()
    {
        GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
        Vector3 direction = (player.position - firePoint.position).normalized;

        direction.z = 0f;
        direction.Normalize();

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();

        if (bulletScript != null)
        {
            bulletScript.playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        }

        if (bulletRb != null)
            bulletRb.linearVelocity = direction * bulletSpeed;

        float angle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg;

        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        enemyhealth -= damage;

        Debug.Log(
            "Special Enemy Health: " + enemyhealth
        );

        if (enemyhealth <= 0)
            Die();
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

        rb.linearVelocity = new Vector3(direction.x * pullSpeed, direction.y * pullSpeed, 0f);
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

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerAirState airState =
                playerObject.GetComponent<PlayerAirState>();

            if (airState != null && airState.IsAirborne)
            {
                StyleMeter.Instance.EnemyKilledInAir();

                Debug.Log("SPECIAL ENEMY AIR KILL!");
            }
        }

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.EnemyDied(this);
        }

        Destroy(gameObject);
    }
}