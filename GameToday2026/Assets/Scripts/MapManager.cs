using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Map Designs")]
    public MapSO[] maps;

    [Header("Floor Tiles")]
    public Transform[] floorTiles;

    [Header("Player Spawn Points")]
    public Transform[] playerSpawnPoints;

    [Header("Starter Map")]
    public int starterMapIndex = 0;

    [Header("Animation")]
    public float transitionDuration = 1f;
    public AnimationCurve transitionCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private int currentMapIndex = -1;
    private bool changingMap;

    void Start()
    {
        SetStarterMap();
    }

    public void SetStarterMap()
    {
        if (maps == null || maps.Length == 0)
        {
            Debug.LogWarning("No maps assigned.");
            return;
        }

        currentMapIndex = starterMapIndex;

        ApplyMapInstant(starterMapIndex);
    }

    public void SetRandomMap()
    {
        if (changingMap)
            return;

        if (maps == null || maps.Length <= 1)
            return;

        int randomIndex;

        do
        {
            randomIndex = Random.Range(0, maps.Length);
        }
        while (randomIndex == currentMapIndex);

        StartCoroutine(ChangeMap(randomIndex));
    }

    IEnumerator ChangeMap(int newIndex)
    {
        changingMap = true;

        MapSO newMap = maps[newIndex];

        Vector3[] startPositions =
            new Vector3[floorTiles.Length];

        Vector3[] targetPositions =
            new Vector3[floorTiles.Length];

        for (int i = 0; i < floorTiles.Length; i++)
        {
            startPositions[i] = floorTiles[i].localPosition;

            targetPositions[i] =
                new Vector3(
                    floorTiles[i].localPosition.x,
                    newMap.tileHeights[i],
                    floorTiles[i].localPosition.z
                );
        }

        float timer = 0f;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;

            float t = timer / transitionDuration;
            t = transitionCurve.Evaluate(t);

            for (int i = 0; i < floorTiles.Length; i++)
            {
                floorTiles[i].localPosition =
                    Vector3.Lerp(
                        startPositions[i],
                        targetPositions[i],
                        t
                    );
            }

            yield return null;
        }

        for (int i = 0; i < floorTiles.Length; i++)
        {
            floorTiles[i].localPosition =
                targetPositions[i];
        }

        currentMapIndex = newIndex;
        changingMap = false;

        Debug.Log("Changed to map: " + maps[newIndex].name);
    }

    void ApplyMapInstant(int mapIndex)
    {
        MapSO map = maps[mapIndex];

        for (int i = 0; i < floorTiles.Length; i++)
        {
            Vector3 position =
                floorTiles[i].localPosition;

            position.y = map.tileHeights[i];

            floorTiles[i].localPosition = position;
        }
    }

    public Transform GetCurrentSpawnPoint()
    {
        if (currentMapIndex < 0 ||
            currentMapIndex >= playerSpawnPoints.Length)
        {
            return null;
        }

        return playerSpawnPoints[currentMapIndex];
    }
}