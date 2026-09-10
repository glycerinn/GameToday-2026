using UnityEngine;

public enum EnemyType { Normal, Fast, Heavy, Shooter }

public class Enemy : MonoBehaviour, IEnemy
{
    [Header("AI Settings")]
    public EnemyType enemyType = EnemyType.Normal;

    [Header("Health")]
    public float maxHealth = 50;
    public float enemyhealth { get; set; }

    [Header("Base Movement")]
    public float moveSpeed = 5f;

    [Header("Normal AI (Smooth Dash)")]
    public float dashSpeed = 15f;
    public float aimDuration = 1.2f;
    public float dashDuration = 0.5f;
    public float normalAcceleration = 8f; // Kecepatan mengerem/ngegas agar smooth
    private float dashTimer;
    private Vector3 dashDirection;
    private bool isDashing = false;

    [Header("Shooter AI (Ultrakill Drone)")]
    public float optimalDistance = 8f;    // Jarak maksimal dari player
    public float retreatDistance = 5f;    // Jika player masuk jarak ini, drone akan mundur
    public float maxChaseDistance = 14f;  // Jika drone lebih jauh dari ini, dia akan mengejar dengan speed boost
    public float catchUpMultiplier = 2f;  // Kelipatan speed saat drone kejauhan dari player
    public float shooterSpeed = 5f;       // Kecepatan maju/mundur
    public float dodgeSpeed = 4f;         // Kecepatan melayang zig-zag
    public float dodgeFrequency = 2f;     // Seberapa cepat drone berganti arah zig-zag
    public float shooterAcceleration = 3f;// Angka kecil = pergerakan semakin smooth (licin melayang)

    [Header("Shooter AI (Weapon)")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float bulletSpeed = 20f;
    public bool leadTarget = true; // prediksi posisi player berdasar kecepatannya
    private float fireTimer;
    private float randomDodgeOffset; // Agar drone yang spawn bersamaan tidak zigzag searah

    [Header("Obstacle & Ground Avoidance (Fast & Shooter)")]
    public LayerMask groundLayer;              // Set ke layer yang dipakai object ber-tag "Ground"
    public float groundCheckDistance = 2f;     // Jarak raycast ke bawah/depan
    public float minHeightAboveGround = 1.5f;  // Jarak minimum di atas lantai sebelum dorong naik
    public float obstacleAvoidSpeed = 10f;     // Kekuatan dorongan menghindar

    [Header("Grapple")]
    public float pullSpeed = 20f;
    public float pullDuration = 0.4f;

    private Transform player;
    private Rigidbody rb;
    private Rigidbody playerRb;
    private bool isDead;
    private bool beingPulled;
    private float pullTimer;

    private AudioManager audioManager;

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager")?.GetComponent<AudioManager>();
    }

    void Start()
    {
        enemyhealth = maxHealth;
        rb = GetComponent<Rigidbody>();

        if (rb == null) return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = playerObject.GetComponent<Rigidbody>();
            Vector3 position = rb.position;
            position.z = player.position.z;
            rb.position = position;
        }

        dashTimer = aimDuration;
        fireTimer = fireRate;
        randomDodgeOffset = Random.Range(0f, 100f);
    }

    void FixedUpdate()
    {
        if (isDead || player == null || rb == null) return;

        if (beingPulled)
        {
            pullTimer -= Time.fixedDeltaTime;
            if (pullTimer <= 0f) StopPull();
        }
        else
        {
            // === STATE MACHINE AI ===
            switch (enemyType)
            {
                case EnemyType.Normal:
                    NormalAI();
                    break;
                case EnemyType.Fast:
                    FastAI();
                    break;
                case EnemyType.Heavy:
                    MoveTowardPlayer();
                    break;
                case EnemyType.Shooter:
                    ShooterAI();
                    break;
            }
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.z = 0f;
        rb.linearVelocity = velocity;
    }

    // === GROUND / OBSTACLE AVOIDANCE (dipakai Fast & Shooter, tipe yang "terbang") ===
    Vector3 GetGroundAvoidance()
    {
        Vector3 avoidance = Vector3.zero;

        // 1. Cek lantai di bawah -> dorong ke atas kalau kejauhan turun
        if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit hitDown, groundCheckDistance, groundLayer))
        {
            if (hitDown.distance < minHeightAboveGround)
            {
                float pushStrength = 1f - (hitDown.distance / minHeightAboveGround);
                avoidance += Vector3.up * pushStrength * obstacleAvoidSpeed;
            }
        }

        // 2. Cek penghalang di arah gerak menuju player -> geser menyamping mengikuti permukaan
        Vector3 forwardDir = player.position - rb.position;
        forwardDir.z = 0f;
        if (forwardDir.sqrMagnitude > 0.01f)
        {
            forwardDir.Normalize();
            if (Physics.Raycast(rb.position, forwardDir, out RaycastHit hitForward, groundCheckDistance, groundLayer))
            {
                Vector3 normal = hitForward.normal;
                normal.z = 0f;
                Vector3 slideDir = Vector3.Cross(normal, Vector3.forward).normalized;
                avoidance += normal * obstacleAvoidSpeed * 0.5f;
                avoidance += slideDir * obstacleAvoidSpeed * 0.5f;
            }
        }

        return avoidance;
    }

    // AI 1: Normal - Mengerem Pelan -> Dash Kencang -> Mengerem Pelan
    void NormalAI()
    {
        dashTimer -= Time.fixedDeltaTime;
        Vector3 targetVelocity = Vector3.zero;

        if (!isDashing)
        {
            // Fase Aiming: Target velocity adalah 0 (Diam)
            if (dashTimer <= 0f)
            {
                dashDirection = (player.position - rb.position).normalized;
                dashDirection.z = 0f;
                dashTimer = dashDuration;
                isDashing = true;
            }
        }
        else
        {
            // Fase Dashing: Target velocity adalah kecepatan penuh
            targetVelocity = dashDirection * dashSpeed;

            if (dashTimer <= 0f)
            {
                dashTimer = aimDuration;
                isDashing = false;
            }
        }

        // Terapkan Lerp agar perubahan dari Diam ke Dash (dan sebaliknya) terasa smooth dan berbobot
        Vector3 currentVel = rb.linearVelocity;
        currentVel.z = 0f;
        rb.linearVelocity = Vector3.Lerp(currentVel, targetVelocity, Time.fixedDeltaTime * normalAcceleration);
    }

    // AI 4: Shooter - Jaga Jarak, Mundur jika didekati, Zigzag Halus, Ada Ground Avoidance
    void ShooterAI()
    {
        Vector3 dirToPlayer = player.position - rb.position;
        dirToPlayer.z = 0f;
        float distance = dirToPlayer.magnitude;
        Vector3 moveDir = dirToPlayer.normalized;

        Vector3 targetVelocity = Vector3.zero;

        // 1. Logika Jarak (Maju & Mundur)
        if (distance > optimalDistance)
        {
            // Kalau kejauhan dari maxChaseDistance, kejar lebih cepat biar gak "kabur" permanen
            float chaseSpeed = distance > maxChaseDistance
                ? shooterSpeed * catchUpMultiplier
                : shooterSpeed;

            targetVelocity += moveDir * chaseSpeed;
        }
        else if (distance < retreatDistance)
        {
            targetVelocity -= moveDir * shooterSpeed; // Terlalu dekat, lari mundur!
        }
        // Jika jarak berada di antara Retreat dan Optimal, drone tidak maju/mundur, hanya melayang.

        // 2. Logika Zigzag / Dodge
        Vector3 sideDir = Vector3.Cross(moveDir, Vector3.forward).normalized;
        float dodgeSway = Mathf.Sin((Time.time * dodgeFrequency) + randomDodgeOffset) * dodgeSpeed;
        targetVelocity += sideDir * dodgeSway;

        // 3. Hindari lantai / obstacle supaya tidak nembus Ground
        targetVelocity += GetGroundAvoidance();

        // 4. Terapkan Lerp untuk pergerakan ala Drone melayang (Hovering)
        Vector3 currentVel = rb.linearVelocity;
        currentVel.z = 0f;
        rb.linearVelocity = Vector3.Lerp(currentVel, targetVelocity, Time.fixedDeltaTime * shooterAcceleration);

        // 5. Logika Menembak
        fireTimer -= Time.fixedDeltaTime;
        if (fireTimer <= 0f)
        {
            TryFire();
            fireTimer = fireRate;
        }
    }

    // Menembak dengan cek line-of-sight ke Ground + prediksi arah gerak player
    void TryFire()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector3 toPlayer = player.position - firePoint.position;
        toPlayer.z = 0f;
        float dist = toPlayer.magnitude;
        if (dist < 0.01f) return;

        Vector3 dirToPlayer = toPlayer.normalized;

        // Jangan tembak kalau ada Ground/tembok menghalangi garis pandang
        if (Physics.Raycast(firePoint.position, dirToPlayer, dist, groundLayer))
            return;

        // Prediksi posisi player berdasarkan kecepatannya biar tembakan lebih akurat
        Vector3 aimDir = dirToPlayer;
        if (leadTarget && playerRb != null)
        {
            float timeToHit = dist / bulletSpeed;
            Vector3 predictedPos = player.position + playerRb.linearVelocity * timeToHit;
            predictedPos.z = firePoint.position.z;

            Vector3 predictedDir = predictedPos - firePoint.position;
            predictedDir.z = 0f;
            if (predictedDir.sqrMagnitude > 0.01f)
                aimDir = predictedDir.normalized;
        }

        GameObject proj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody projRb = proj.GetComponent<Rigidbody>();
        if (projRb != null) projRb.linearVelocity = aimDir * bulletSpeed;
    }

    // AI 2: Terbang agresif berputar mendekati player, Ada Ground Avoidance
    void FastAI()
    {
        Vector3 dirToPlayer = (player.position - rb.position);
        dirToPlayer.z = 0f;
        Vector3 moveDir = dirToPlayer.normalized;
        Vector3 cross = Vector3.Cross(moveDir, Vector3.forward);
        Vector3 finalDir = (moveDir + (cross * 1.5f)).normalized;

        Vector3 targetVelocity = finalDir * moveSpeed;
        targetVelocity += GetGroundAvoidance();

        rb.linearVelocity = targetVelocity;
    }

    // AI 3: Heavy Base Movement (TIDAK BERUBAH)
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
        if (isDead) return;
        enemyhealth -= damage;
        if (enemyhealth <= 0f) Die();
    }

    // === LOGIKA GRAPPLING HOOK ===
    public bool OnGrappled()
    {
        if (enemyType == EnemyType.Heavy || enemyType == EnemyType.Shooter)
            return false;

        PullToPlayer();
        return true;
    }

    public void PullToPlayer()
    {
        if (player == null || rb == null) return;
        Vector3 direction = player.position - rb.position;
        direction.z = 0f;
        if (direction.sqrMagnitude < 0.01f) return;

        direction.Normalize();
        beingPulled = true;
        pullTimer = pullDuration;
        rb.linearVelocity = new Vector3(direction.x * pullSpeed, direction.y * pullSpeed, 0f);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (audioManager != null) audioManager.playDieSFX();
        if (StyleMeter.Instance != null) StyleMeter.Instance.EnemyKilled();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerAirState airState = playerObject.GetComponent<PlayerAirState>();
            if (airState != null && airState.IsAirborne)
            {
                StyleMeter.Instance.EnemyKilledInAir();
                Debug.Log("AIR KILL! +2 STYLE");
            }
        }

        if (WaveManager.Instance != null) WaveManager.Instance.EnemyDied(this);
        Destroy(gameObject);
    }

    void StopPull()
    {
        beingPulled = false;
        rb.linearVelocity = Vector3.zero;
    }
}
