using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun")]
    private float cooldown = 1f;
    private float currentcooldown;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [Header("Aim")]
    public Transform hammertip;

    [Header("Bullet")]
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;

    [Header("Ammo")]
    public int bulletcount = 0;
    public int tilReload = 5;
    public bool isReloading = false;

    void Update()
    {
        if (UpgradeManager.UpgradeSelectionActive)
            return;

        if (isReloading)
            return;

        currentcooldown -= Time.deltaTime;

        if (currentcooldown <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        FireGun();

        currentcooldown = cooldown;

        bulletcount++;
        tilReload--;

        if (tilReload <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(1f);

        tilReload = 5;
        isReloading = false;
    }

    private void FireGun()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        Vector3 direction = hammertip.up;

        // Force the bullet to stay on the XY plane
        direction.z = 0f;
        direction.Normalize();

        bulletRb.linearVelocity = direction * bulletVelocity;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        StartCoroutine(DestroyBullet(bullet, bulletPrefabLifeTime));
    }

    private IEnumerator DestroyBullet(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bullet != null)
        {
            Destroy(bullet);
        }
    }
}