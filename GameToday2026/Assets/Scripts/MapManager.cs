using System.Collections;
using System.Collections.Generic;
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

    // Maps that have not been used yet
    private List<int> availableMaps = new List<int>();


    void Start()
    {
        SetStarterMap();

        SetupAvailableMaps();
    }


    public void SetStarterMap()
    {
        if (maps == null || maps.Length == 0)
        {
            Debug.LogWarning("No maps assigned.");
            return;
        }

        if (starterMapIndex < 0 ||
            starterMapIndex >= maps.Length)
        {
            Debug.LogWarning("Invalid starter map index.");
            return;
        }

        currentMapIndex = starterMapIndex;

        ApplyMapInstant(starterMapIndex);
    }


    void SetupAvailableMaps()
    {
        availableMaps.Clear();

        for (int i = 0; i < maps.Length; i++)
        {
            // Don't add the starter map because
            // it has already been used.
            if (i != currentMapIndex)
            {
                availableMaps.Add(i);
            }
        }
    }


    public void SetRandomMap()
    {
        if (changingMap)
            return;

        if (maps == null || maps.Length <= 1)
            return;


        // If every map has been used,
        // reset the available map list.
        if (availableMaps.Count == 0)
        {
            Debug.Log("All maps have been used. Resetting map pool.");

            SetupAvailableMaps();
        }


        int randomListIndex =
            Random.Range(0, availableMaps.Count);

        int randomMapIndex =
            availableMaps[randomListIndex];


        // Remove this map so it cannot be used again
        availableMaps.RemoveAt(randomListIndex);


        StartCoroutine(ChangeMap(randomMapIndex));
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
            startPositions[i] =
                floorTiles[i].localPosition;

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

        Debug.Log(
            "Changed to map: " +
            maps[newIndex].name
        );
    }


    void ApplyMapInstant(int mapIndex)
    {
        MapSO map = maps[mapIndex];


        for (int i = 0; i < floorTiles.Length; i++)
        {
            Vector3 position =
                floorTiles[i].localPosition;

            position.y =
                map.tileHeights[i];

            floorTiles[i].localPosition =
                position;
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