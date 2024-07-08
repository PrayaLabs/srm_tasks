using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public GameObject[] collectibles;
    public int collectibleCount = 20;

    void Start()
    {
        for (int i = 0; i < collectibleCount; i++)
        {
            SpawnCollectible();
        }
    }

    void SpawnCollectible()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(-10,11),
            4,
            Random.Range(-10,11)
        );

        GameObject collectible = collectibles[Random.Range(0, collectibles.Length)];
        Instantiate(collectible, spawnPosition, Quaternion.identity);
    }
}
