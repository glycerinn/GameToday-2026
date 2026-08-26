using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    public float spawnWidth = 15f;
    public float spawnHeight = 8f;

    [Header("Player")]
    public Transform player;
    public float minimumPlayerDistance = 5f;

    public List<IEnemy> SpawnEnemies(EnemySpawnData[] spawnData)
    {
        List<IEnemy> spawnedEnemies = new List<IEnemy>();

        foreach (EnemySpawnData data in spawnData)
        {
            if (data.enemyPrefab == null)
            {
                Debug.LogWarning("Enemy spawn entry has no prefab assigned.");
                continue;
            }

            for (int i = 0; i < data.amount; i++)
            {
                IEnemy enemy = SpawnEnemy(data.enemyPrefab);

                if (enemy != null)
                {
                    spawnedEnemies.Add(enemy);
                }
            }
        }

        return spawnedEnemies;
    }

    IEnemy SpawnEnemy(GameObject prefab)
    {
        Vector3 spawnPosition;
        int attempts = 0;

        do
        {
            float randomX =
                Random.Range(-spawnWidth / 2f, spawnWidth / 2f);

            float randomY =
                Random.Range(-spawnHeight / 2f, spawnHeight / 2f);

            spawnPosition =
                transform.position +
                new Vector3(randomX, randomY, 0f);

            attempts++;

        } while (
            Vector3.Distance(
                spawnPosition,
                player.position
            ) < minimumPlayerDistance
            &&
            attempts < 100
        );

        spawnPosition.z = player.position.z;

        GameObject enemyObject =
            Instantiate(
                prefab,
                spawnPosition,
                Quaternion.identity
            );

        IEnemy enemy =
            enemyObject.GetComponent<IEnemy>();

        if (enemy == null)
        {
            Debug.LogError(
                "Enemy prefab does not implement IEnemy!",
                enemyObject
            );

            Destroy(enemyObject);
            return null;
        }

        return enemy;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(
                spawnWidth,
                spawnHeight,
                0.1f
            )
        );

        if (player != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                player.position,
                minimumPlayerDistance
            );
        }
    }
}