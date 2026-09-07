using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public PlayerHealth playerHealth;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.takeDamage(10);
            Destroy(gameObject);
        }
    }
}