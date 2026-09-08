public interface IEnemy
{
    float enemyhealth { get; set; }
    void TakeDamage(float damage);
    void Die();
    void PullToPlayer();
}