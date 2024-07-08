using UnityEngine;

public class AdvancedPlayer : Player
{
    public float doubleJumpForce = 5f;
    private bool canDoubleJump;

    void Update()
    {
        Move();
        Jump();
    }

    new void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Single Jump");
            PlayerRb.AddForce(Vector3.up *Force, ForceMode.Impulse);
            canDoubleJump = true;
        }
        else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Double Jump");
            PlayerRb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
            canDoubleJump = false;
        }
    }
}
