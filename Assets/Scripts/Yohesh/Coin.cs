using System.Collections;
using UnityEngine;

public class Coin : Collectible
{
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.name=="Player") 
        {
            Collect();
            
            Destroy(gameObject);

        }
    }
}
