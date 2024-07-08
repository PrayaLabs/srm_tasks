using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected Transform player;
    public float speed = 3f;

    public abstract void Move();
    public abstract void Attack();
    void update() {
    }

    protected void Update()
    {
        Move();
    }

  
    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }
}
