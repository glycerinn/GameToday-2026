using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Maps")]
    public GameObject[] maps;

    [Header("Player Spawn Points")]
    public Transform[] playerSpawnPoints;

    [Header("Starter Map")]
    public int starterMapIndex = 0;

    private int currentMapIndex = -1;

    void Start()
    {
        SetStarterMap();
    }

    public void SetStarterMap()
    {
        if (maps == null || maps.Length == 0)
        {
            Debug.LogWarning("No maps assigned to MapManager.");
            return;
        }

        if (starterMapIndex < 0 || starterMapIndex >= maps.Length)
        {
            Debug.LogWarning("Starter map index is invalid.");
            return;
        }

        SetMap(starterMapIndex);
    }

    public void SetRandomMap()
    {
        if (maps == null || maps.Length == 0)
        {
            Debug.LogWarning("No maps assigned to MapManager.");
            return;
        }

        int randomIndex;

        if (maps.Length == 1)
        {
            randomIndex = 0;
        }
        else
        {
            do
            {
                randomIndex = Random.Range(0, maps.Length);
            }
            while (randomIndex == currentMapIndex);
        }

        SetMap(randomIndex);
    }

    void SetMap(int index)
    {
        for (int i = 0; i < maps.Length; i++)
        {
            if (maps[i] != null)
                maps[i].SetActive(i == index);
        }

        currentMapIndex = index;

        Debug.Log("MAP SELECTED: " + maps[index].name);
    }

    public Transform GetCurrentSpawnPoint()
    {
        if (playerSpawnPoints == null ||
            currentMapIndex < 0 ||
            currentMapIndex >= playerSpawnPoints.Length)
        {
            return null;
        }

        return playerSpawnPoints[currentMapIndex];
    }
}