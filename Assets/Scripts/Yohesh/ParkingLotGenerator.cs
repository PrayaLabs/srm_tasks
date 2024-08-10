using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParkingLotGenerator : MonoBehaviour
{
    public GameObject parkingSlotPrefab; 
    public int rows = 10; 
    public int columns = 10; 
    public float slotSpacingX = 3f; 
    public float slotSpacingZ = 6f; 
    public float roadWidth = 4f; 
    public UILoad1 uiLoadScript; 

    private List<ParkingSlotBorderController> generatedSlots = new List<ParkingSlotBorderController>();

    void Start()
    {
        GenerateParkingLot();
        uiLoadScript.parkingSlots = generatedSlots.ToArray(); 
    }

    void GenerateParkingLot()
    {
        int slotNumber = 1;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float xOffset = col * slotSpacingX;
                float zOffset = row * (slotSpacingZ + roadWidth);

                Vector3 position = new Vector3(xOffset, 0, zOffset);
                GameObject slot = Instantiate(parkingSlotPrefab, position, Quaternion.identity, transform);

               
                TextMeshPro textMeshPro = slot.GetComponentInChildren<TextMeshPro>();
                if (textMeshPro != null)
                {
                    textMeshPro.text = slotNumber.ToString(); 
                }
                else
                {
                    Debug.LogError("TextMeshPro component not found in the parking slot prefab.");
                }

                ParkingSlotBorderController slotController = slot.GetComponent<ParkingSlotBorderController>();
                if (slotController != null)
                {
                    generatedSlots.Add(slotController);
                    slot.name = "Slot_" + slotNumber; 
                }
                else
                {
                    Debug.LogError("ParkingSlotBorderController component not found in the parking slot prefab.");
                }

                slotNumber++;
            }
        }
    }
}
