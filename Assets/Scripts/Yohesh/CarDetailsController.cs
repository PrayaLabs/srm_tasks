using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class CarDetailsController : MonoBehaviour
{
    public Transform carPrefabPlaceholder;
    public TMP_Text carDetailsText;        
    public Button backButton;             

    // Updated vehicle prefabs
    public GameObject pickupPrefab;
    public GameObject sedanPrefab;
    public GameObject suvPrefab;
    public GameObject coupePrefab;
    public GameObject hatchbackPrefab;
    public GameObject wagonPrefab;
    public GameObject minivanPrefab;
    public GameObject vanPrefab;
    public GameObject jeepPrefab;
    public GameObject convertiblePrefab;

    private Dictionary<string, GameObject> vehiclePrefabs;

    void Start()
    {
        InitializeVehiclePrefabs();

        // Load the selected slot data from PlayerPrefs
        string jsonSlot = PlayerPrefs.GetString("SelectedSlot", "");
        if (!string.IsNullOrEmpty(jsonSlot))
        {
            ParkingSlotData selectedSlot = JsonUtility.FromJson<ParkingSlotData>(jsonSlot);
            DisplayCarDetails(selectedSlot);
        }
        else
        {
            Debug.LogError("SelectedSlot data not found in PlayerPrefs.");
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainParking");
            });
        }
    }

    private void InitializeVehiclePrefabs()
    {
        vehiclePrefabs = new Dictionary<string, GameObject>
        {
            { "pickup", pickupPrefab },
            { "sedan", sedanPrefab },
            { "suv", suvPrefab },
            { "coupe", coupePrefab },
            { "hatchback", hatchbackPrefab },
            { "wagon", wagonPrefab },
            { "minivan", minivanPrefab },
            { "van", vanPrefab },
            { "jeep", jeepPrefab },
            { "convertible", convertiblePrefab }
        };
    }

    private void DisplayCarDetails(ParkingSlotData slot)
    {
        if (slot == null)
        {
            Debug.LogError("Slot data is null. Cannot display car details.");
            return;
        }

        if (vehiclePrefabs == null)
        {
            Debug.LogError("VehiclePrefabs dictionary is not initialized. Ensure InitializeVehiclePrefabs is called.");
            return;
        }

        if (string.IsNullOrEmpty(slot.CarModel))
        {
            Debug.LogError("CarModel is null or empty in the selected slot data.");
            return;
        }

        
        string carModelKey = slot.CarModel.Replace("(Clone)", "").Trim().ToLower();

        if (vehiclePrefabs.ContainsKey(carModelKey))
        {
            foreach (Transform child in carPrefabPlaceholder)
            {
                Destroy(child.gameObject);
            }

            GameObject carPrefab = vehiclePrefabs[carModelKey];
            Quaternion rotation = Quaternion.Euler(0f, 90f, 0f); 
            Instantiate(carPrefab, carPrefabPlaceholder.position, rotation, carPrefabPlaceholder);
        }
        else
        {
            Debug.LogError($"No prefab found for car model: {slot.CarModel}. Check if it matches the dictionary keys.");
        }

        // Update car details text
        if (carDetailsText != null)
        {
            carDetailsText.text = $"Slot: {slot.SlotNumber}\nOwner Name: {slot.Name}\nCar Model: {slot.CarModel.Replace("(Clone)", "").Trim().ToLower()}\nVehicle Number: {slot.VehicleNumber}";
        }
        else
        {
            Debug.LogError("carDetailsText is not assigned in the Editor.");
        }
    }
}

[System.Serializable]
public class ParkingSlotData
{
    public string Name;           
    public string CarModel;        
    public string VehicleNumber;   
    public string SlotNumber;      
}
