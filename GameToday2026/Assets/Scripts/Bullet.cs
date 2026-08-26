using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        IEnemy enemy = collision.gameObject.GetComponentInParent<IEnemy>();

        if (enemy != null)
        {
            enemy.Die();
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}