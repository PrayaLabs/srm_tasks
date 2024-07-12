using UnityEngine;

public class EvenSpread : MonoBehaviour
{
    public GameObject Object; 
    public int Count = 10;
    public float dis = 2.0f; 

    void Start()
    {
        SpreadObjects();
    }

    void SpreadObjects()
    {
        for (int i = 0; i < Count; i++)
        {
            Vector3 position = new Vector3(i * dis, 0, 0);

            Instantiate(Object, position, Quaternion.identity);
        }
    }
}
