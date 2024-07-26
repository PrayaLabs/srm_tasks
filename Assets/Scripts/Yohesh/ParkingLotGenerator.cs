using UnityEngine;

public class ParkingLotGenerator : MonoBehaviour
{
    public GameObject parkingSlotPrefab;
    public int rows = 10; 
    public int columns = 10;
    public float slotSpacingX = 3f; 
    public float slotSpacingZ = 6f; 
    public float roadWidth = 4f; 

    void Start()
    {
        GenerateParkingLot();
    }

    void GenerateParkingLot()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float xOffset = col * slotSpacingX;
                float zOffset = row * (slotSpacingZ + roadWidth);

                Vector3 position = new Vector3(xOffset, 0, zOffset);
                Instantiate(parkingSlotPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}