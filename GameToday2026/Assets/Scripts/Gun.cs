using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [Header("Gun")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [Header("Aim")]
    public Transform hammertip;

    [Header("Bullet")]
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;

    [Header("Default Gun Spread")]
    public int bulletsPerShot = 5;
    public float spreadAngle = 15f;

    [Header("Player Knockback")]
    public float playerKnockbackForce = 0.5f;
    public Rigidbody playerRb;

    [Header("Gun Modes")]
    public bool secondGunModeUnlocked = false;
    public bool secondGunModeActive = false;

    [Header("Charge")]
    public float defaultChargeTime = 1f;
    public float secondChargeTime = 0.5f;
    [Range(0f, 1f)]
    public float charge = 1f;

    [Header("Charge UI")]
    public Slider chargeBar;

    [Header("Time Stop Mechanic")]
    public float timeStopDuration = 1.0f; // Berapa lama waktu membeku (detik)
    [Range(0f, 1f)]
    public float timeStopScale = 0.02f;   // Seberapa lambat gamenya (0 = berhenti total)
    private bool isTimeStopped = false;

    void Start()
    {
        charge = 1f;

        if (chargeBar != null)
        {
            chargeBar.minValue = 0f;
            chargeBar.maxValue = 1f;
            chargeBar.value = charge;
        }
    }

    void Update()
    {
        if (UpgradeManager.UpgradeSelectionActive || WaveManager.DialogueActive)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetGunMode(1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetGunMode(2);

        Recharge();

        // Cek juga agar pemain tidak bisa menembak berulang kali saat waktu sedang membeku
        if (Input.GetKeyDown(KeyCode.Mouse0) && charge >= 1f && !isTimeStopped)
            Shoot();
    }

    private void Recharge()
    {
        // Jangan isi ulang peluru jika sedang mode time stop
        if (charge >= 1f || isTimeStopped)
            return;

        float rechargeTime = secondGunModeActive
            ? secondChargeTime
            : defaultChargeTime;

        // Gunakan unscaledDeltaTime agar charge tetap berjalan normal meski game sedang Slow-Mo
        charge += Time.unscaledDeltaTime / rechargeTime;
        charge = Mathf.Clamp01(charge);

        if (chargeBar != null)
            chargeBar.value = charge;
    }

    private void Shoot()
    {
        charge = 0f;

        if (chargeBar != null)
            chargeBar.value = charge;

        // Jalankan efek Time Stop alih-alih menembak instan
        StartCoroutine(TimeStopShootRoutine());
    }

    private IEnumerator TimeStopShootRoutine()
    {
        isTimeStopped = true;

        // 1. Tembakkan peluru (Peluru akan melayang super lambat karena timeScale 0.02)
        FireGunBulletsOnly();

        // 2. Hentikan waktu
        Time.timeScale = timeStopScale;

        // 3. Beri jeda agar pemain bisa mengarahkan palu ke posisi baru
        yield return new WaitForSecondsRealtime(timeStopDuration);

        // 4. Kembalikan waktu ke normal
        Time.timeScale = 1f;

        // 5. Berikan efek knockback dari arah palu yang TERBARU
        ApplyDelayedKnockback();

        isTimeStopped = false;
    }

    private void FireGunBulletsOnly()
    {
        Vector3 baseDirection = hammertip.up;
        baseDirection.z = 0f;
        baseDirection.Normalize();

        int bulletAmount = secondGunModeActive ? 1 : bulletsPerShot;

        for (int i = 0; i < bulletAmount; i++)
        {
            Vector3 direction = baseDirection;

            if (bulletAmount > 1)
            {
                float angleStep = spreadAngle / (bulletAmount - 1);
                float angleOffset = -spreadAngle / 2f + angleStep * i;
                direction = Quaternion.Euler(0f, 0f, angleOffset) * baseDirection;
            }

            CreateBullet(direction);
        }
    }

    private void ApplyDelayedKnockback()
    {
        // Jika mode kedua aktif (No Knockback) atau Rb kosong, jangan beri dorongan
        if (secondGunModeActive || playerRb == null)
            return;

        // Ambil arah palu TERBARU setelah jeda waktu habis
        Vector3 newDirection = hammertip.up;
        newDirection.z = 0f;
        newDirection.Normalize();

        // (Opsional) Netralkan kecepatan jatuh agar dorongan ke atas tidak terasa berat
        Vector3 currentVel = playerRb.linearVelocity;
        if (currentVel.y < 0) currentVel.y = 0;
        playerRb.linearVelocity = currentVel;

        playerRb.AddForce(-newDirection * playerKnockbackForce, ForceMode.Impulse);
    }

    private void CreateBullet(Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = direction * bulletVelocity;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
            bulletScript.playerHealth = playerRb.GetComponent<PlayerHealth>();

        StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay); // Tetap pakai WaitForSeconds biasa agar timer peluru ikut melambat

        if (bullet != null)
            Destroy(bullet);
    }

    // ==========================================
    // FUNGSI BAWAAN ANDA (TIDAK DIHAPUS)
    // ==========================================

    public void UnlockSecondGunMode()
    {
        secondGunModeUnlocked = true;
        Debug.Log("SECOND GUN MODE UNLOCKED!");
    }

    public void SetGunMode(int mode)
    {
        if (mode == 1)
        {
            secondGunModeActive = false;
            Debug.Log("Gun Mode 1: Default");
        }
        else if (mode == 2)
        {
            if (!secondGunModeUnlocked)
                return;

            secondGunModeActive = true;
            Debug.Log("Gun Mode 2: No Knockback");
        }
    }

    public void IncreaseKnockback(float amount)
    {
        playerKnockbackForce += amount;
        Debug.Log("Knockback increased by " + amount + ". New knockback: " + playerKnockbackForce);
    }

    public void DecreaseChargeTime(float amount)
    {
        defaultChargeTime -= amount;
        secondChargeTime -= amount;

        defaultChargeTime = Mathf.Max(0.05f, defaultChargeTime);
        secondChargeTime = Mathf.Max(0.05f, secondChargeTime);

        Debug.Log("Charge times decreased by " + amount + ". Default: " + defaultChargeTime + ", Second: " + secondChargeTime);
    }
}