using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject groundEnemyPrefab; 
    public GameObject flyingEnemyPrefab; 
    public Transform player; 
    public float spawnInterval = 2f; 

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
       //fr or gr spawn
        bool spawnGroundEnemy = Random.value > 0.5f;
        GameObject enemyPrefab = spawnGroundEnemy ? groundEnemyPrefab : flyingEnemyPrefab;

        //pos random
        Vector3 spawnPosition = player.position + new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        
        enemy.GetComponent<Enemy>().SetPlayer(player);
    }
}
