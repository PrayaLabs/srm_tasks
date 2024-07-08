using UnityEngine;

public class GroundEnemy : Enemy
{
    protected override void Move()
    {
        // Ground enemy moves towards the player (same as base)
        base.Move();
    }

    public override void Attack()
    {
        // Custom attack for ground enemy
        Debug.Log("Ground enemy attacks with a slam for " + damage + " damage.");
    }
}

