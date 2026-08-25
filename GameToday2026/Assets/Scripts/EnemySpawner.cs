using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Spawn Area")]
    public float spawnWidth = 15f;
    public float spawnHeight = 8f;

    [Header("Player")]
    public Transform player;
    public float minimumPlayerDistance = 5f;

    public void SpawnEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition;
        int attempts = 0;

        do
        {
            float randomX = Random.Range( -spawnWidth / 2f, spawnWidth / 2f);
            float randomY = Random.Range(-spawnHeight / 2f, spawnHeight / 2f);
            spawnPosition = transform.position + new Vector3(randomX, randomY, 0f);

            attempts++;

        } while (
            Vector3.Distance(spawnPosition, player.position) < minimumPlayerDistance && attempts < 100
        );

        // Force enemy onto player's Z axis
        spawnPosition.z = player.position.z;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnWidth, spawnHeight,0.1f));

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minimumPlayerDistance);
        }
    }
}