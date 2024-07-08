using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject flyingEnemyPrefab;
    public GameObject groundEnemyPrefab;

    public float spawnInterval = 5f;
    public float spawnRange = 10f;

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange, spawnRange), 0.5f, Random.Range(-spawnRange, spawnRange));
            GameObject enemy = Instantiate(SelectRandomEnemyPrefab(), spawnPosition, Quaternion.identity);
        }
    }

    GameObject SelectRandomEnemyPrefab()
    {
        int randomValue = Random.Range(0, 3);
        switch (randomValue)
        {
            case 0:
                return enemyPrefab;
            case 1:
                return flyingEnemyPrefab;
            case 2:
                return groundEnemyPrefab;
            default:
                return enemyPrefab;
        }
    }
}

