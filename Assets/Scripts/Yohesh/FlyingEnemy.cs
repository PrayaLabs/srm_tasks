using UnityEngine;

public class FlyingEnemy : Enemy
{
    public override void Move()
    {
        

        
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        
        transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time) + 5, transform.position.z);
    }

    public override void Attack()
    {
        Debug.Log("Flying enemy attacks!");
       
    }
}