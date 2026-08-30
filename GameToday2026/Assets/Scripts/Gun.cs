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
        if (UpgradeManager.UpgradeSelectionActive)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetGunMode(1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetGunMode(2);

        Recharge();

        if (Input.GetKeyDown(KeyCode.Mouse0) && charge >= 1f)
            Shoot();
    }

    private void Recharge()
    {
        if (charge >= 1f)
            return;

        float rechargeTime = secondGunModeActive
            ? secondChargeTime
            : defaultChargeTime;

        charge += Time.deltaTime / rechargeTime;
        charge = Mathf.Clamp01(charge);

        if (chargeBar != null)
            chargeBar.value = charge;
    }

    private void Shoot()
    {
        FireGun();

        charge = 0f;

        if (chargeBar != null)
            chargeBar.value = charge;
    }

    private void FireGun()
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

        if (!secondGunModeActive && playerRb != null)
            playerRb.AddForce(-baseDirection * playerKnockbackForce, ForceMode.Impulse);
    }

    private void CreateBullet(Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = direction * bulletVelocity;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bullet != null)
            Destroy(bullet);
    }

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
}