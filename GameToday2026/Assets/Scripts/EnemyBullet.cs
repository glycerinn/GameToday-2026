using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public PlayerHealth playerHealth;

    [Header("Balancing Settings")]
    public float bulletDamage = 15f; // Ubah damage tembakan musuh di sini

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Ambil PlayerHealth otomatis jika belum di-assign
            if (playerHealth == null)
                playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
                playerHealth.takeDamage(bulletDamage);

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
            Destroy(gameObject);
    }
}