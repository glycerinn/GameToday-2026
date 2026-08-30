using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float healOnEnemyKill = 25f;
    public EnemySpawner enemySpawner;
    public float health { get; set; }

    public Slider healthSlider;
    public float enemyCollisionDamage = 20f;

    void Start()
    {
        health = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        UpdateHealthUI();
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        healthSlider.value = health;
        UpdateHealthUI();

        if (health <= 0)
        {
            enemySpawner.enabled = false;
            Time.timeScale = 0f;
            Debug.Log("Player Died");
        }
    }

    public void HealOnEnemyKill()
    {
        Heal(healOnEnemyKill);
    }

    public void IncreaseKillHeal(float amount)
    {
        healOnEnemyKill += amount;

        Debug.Log("Enemy kill heal increased by " + amount + ". New heal: " + healOnEnemyKill);
    }

    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0f, maxHealth);
        UpdateHealthUI();

        Debug.Log("Player healed for " + amount + ". Current health: " + health);
    }

    void UpdateHealthUI()
    {
        healthSlider.value = health;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IEnemy enemy = collision.gameObject.GetComponentInParent<IEnemy>();

        if (enemy == null)
            return;

        if (collision.contacts.Length == 0)
            return;

        Collider hitCollider = collision.contacts[0].thisCollider;

        if (!hitCollider.CompareTag("Player"))
            return;

        Debug.Log("PLAYER COLLIDED WITH ENEMY!");

        takeDamage(enemyCollisionDamage);

        enemy.Die();
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        health += amount;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        UpdateHealthUI();

        Debug.Log(
            "Max health increased by " +
            amount +
            ". New max health: " +
            maxHealth
        );
    }
}