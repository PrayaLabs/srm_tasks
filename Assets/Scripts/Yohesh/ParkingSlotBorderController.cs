using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ParkingSlotBorderController : MonoBehaviour
{
    public Renderer[] borderRenderers; 
    public TextMeshPro vehicleInfoText; 
    public Transform vehicleSpawnPoint; 
    public Material availableMaterial; 
    public Material occupiedMaterial;

    public Vector3 vehicleRotation = Vector3.zero; 
    public Vector3 fixedPositionOffset = Vector3.zero; 

    private GameObject currentVehicle; 

   
    private HashSet<string> fixedRotationModels = new HashSet<string>() //fixed car models rotation as its aligned at point
    {
        "van",
        "suv",
        "jeep"
    };

    public void SetAvailability(bool available, string vehicleInfo = "")
    {
        Material selectedMaterial = available ? availableMaterial : occupiedMaterial;

        foreach (var borderRenderer in borderRenderers)
        {
            if (borderRenderer != null)
            {
                borderRenderer.material = selectedMaterial;
                borderRenderer.material.EnableKeyword("_EMISSION");
                borderRenderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                DynamicGI.SetEmissive(borderRenderer, selectedMaterial.color);
            }
        }

        if (vehicleInfoText != null)
        {
            vehicleInfoText.text = vehicleInfo;
        }

        if (!available && !string.IsNullOrEmpty(vehicleInfo))
        {
            
        }
        else
        {
            RemoveVehicle();
        }
    }

    public void SpawnSpecificVehicle(GameObject vehiclePrefab)
    {
        RemoveVehicle();

        if (vehiclePrefab != null && vehicleSpawnPoint != null)
        {
            
            string carModel = vehiclePrefab.name.ToLower();

            if (fixedRotationModels.Contains(carModel))
            {
                
                Vector3 spawnPosition = vehicleSpawnPoint.position + fixedPositionOffset;
                currentVehicle = Instantiate(vehiclePrefab, spawnPosition, Quaternion.identity);
            }
            else
            {
                
                currentVehicle = Instantiate(vehiclePrefab, vehicleSpawnPoint.position, Quaternion.Euler(vehicleRotation));
            }
        }
    }

    private void RemoveVehicle()
    {
        if (currentVehicle != null)
        {
            Destroy(currentVehicle);
            currentVehicle = null;
        }
    }
}