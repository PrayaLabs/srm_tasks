using UnityEngine;

public class GroundEnemy : Enemy
{
    public override void Move()
    {
       

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    public override void Attack()
    {
        Debug.Log("Ground enemy attacks!");
    }
}