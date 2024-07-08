using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float health = 100f;
    static public float speed = 5f;
    public float Force = 7f;

    protected Rigidbody PlayerRb;
    protected bool isGrounded;

    void Start()
    {
        PlayerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        Jump();
       
    }

    protected void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * speed * moveVertical);
        transform.Translate(Vector3.right * Time.deltaTime * speed * moveHorizontal);
    }

    protected void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            PlayerRb.AddForce(Vector3.up * Force,ForceMode.Impulse);

        }
    }



    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Grounded");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Not Grounded");
        }
    }
}
