using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value;
    public string type;

    public virtual void Collect()
    {
        Debug.Log($"Collected a {type}.");
    }
}
