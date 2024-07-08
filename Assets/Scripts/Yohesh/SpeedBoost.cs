using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpeedBoost : Collectible
{
    public float multiplier = 10f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            Player.speed *= multiplier;
            Collect();
            Destroy(gameObject);

        }

    }
}
