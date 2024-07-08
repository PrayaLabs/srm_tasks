using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float damage = 10f;
    public float speed = 2f;

    protected Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        // Basic movement towards the player
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
    }

    public virtual void Attack()
    {
        // Basic attack logic
        Debug.Log("Enemy attacks for " + damage + " damage.");
    }
}

