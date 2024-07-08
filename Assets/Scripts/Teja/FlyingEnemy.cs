using UnityEngine;

public class FlyingEnemy : Enemy
{
    public float flightHeight = 5f;

    protected override void Move()
    {
        // Fly towards the player
        Vector3 targetPosition = new Vector3(playerTransform.position.x, flightHeight, playerTransform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public override void Attack()
    {
        // Custom attack for flying enemy
        Debug.Log("Flying enemy swoops down for " + damage + " damage.");
    }
}

