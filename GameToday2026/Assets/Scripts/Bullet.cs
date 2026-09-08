using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 25f;
    public PlayerHealth playerHealth;

    private void OnCollisionEnter(Collision collision)
    {
        IEnemy enemy = collision.gameObject.GetComponentInParent<IEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            if (playerHealth != null)
                playerHealth.HealOnEnemyKill();

            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
            Destroy(gameObject);
    }
}