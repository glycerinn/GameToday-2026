public interface IEnemy
{
    float enemyhealth { get; set; }
    void TakeDamage(float damage);
    void Die();
    void PullToPlayer();

    // Fungsi baru untuk menentukan siapa yang ditarik saat kena Grappling Hook
    bool OnGrappled();
}