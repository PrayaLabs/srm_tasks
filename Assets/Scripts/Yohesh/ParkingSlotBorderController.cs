using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    // Track availability and vehicle number
    public bool IsAvailable { get; private set; } = true;
    public string VehicleNumber { get; private set; }

    private HashSet<string> fixedRotationModels = new HashSet<string>()
    {
        "van",
        "suv",
        "jeep"
    };

    // Getter for current vehicle
    public GameObject GetCurrentVehicle()
    {
        return currentVehicle;
    }

    // Method to set the availability of the parking slot and update material & vehicle info
    public void SetAvailability(bool available, string vehicleInfo = "", string vehicleNumber = null)
    {
        IsAvailable = available;
        VehicleNumber = vehicleNumber;

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
            // Keep the vehicle if occupied
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

            // Apply a vertical offset to the spawn position to raise the vehicle
            Vector3 spawnPosition = vehicleSpawnPoint.position + fixedPositionOffset;
            spawnPosition.y += 0.2f; // Adjust this value to control how much higher the vehicle spawns (e.g., 1.0f)

            if (fixedRotationModels.Contains(carModel))
            {
                currentVehicle = Instantiate(vehiclePrefab, spawnPosition, Quaternion.identity);
            }
            else
            {
                currentVehicle = Instantiate(vehiclePrefab, spawnPosition, Quaternion.Euler(vehicleRotation));
            }
        }
    }


    // Remove the vehicle from the parking slot
    private void RemoveVehicle()
    {
        if (currentVehicle != null)
        {
            Destroy(currentVehicle);
            currentVehicle = null;
        }
    }
}
