using UnityEngine;

public class Bullet : MonoBehaviour
{
    public PlayerHealth playerHealth;

    private void OnCollisionEnter(Collision collision)
    {
        IEnemy enemy = collision.gameObject.GetComponentInParent<IEnemy>();

        if (enemy != null)
        {
            enemy.Die();

            if (playerHealth != null)
                playerHealth.HealOnEnemyKill();

            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
            Destroy(gameObject);
    }
}