using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public GameOverPanel GamePanel;
    public float healOnEnemyKill = 25f;
    public EnemySpawner enemySpawner;
    public float health;

    public Slider healthSlider;
    public float enemyCollisionDamage = 20f;

    public static bool GameOver { get; private set; }

    void Start()
    {
        health = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;

        UpdateHealthUI();
    }

    public void takeDamage(float damage)
    {
        if (GameOver)
            return;
        health -= damage;
        healthSlider.value = health;
        UpdateHealthUI();

        if (health <= 0)
        {
            GameOver = true;
            enemySpawner.enabled = false;
            GamePanel.ShowLose();
            // gameObject.SetActive(false);

            Debug.Log("Player Died");

            Time.timeScale = 0f;
        }
    }

    public void HealOnEnemyKill()
    {
        Heal(healOnEnemyKill);
    }

    public void IncreaseKillHeal(float amount)
    {
        healOnEnemyKill += amount;
    }

    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0f, maxHealth);
        UpdateHealthUI();
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
    }
}