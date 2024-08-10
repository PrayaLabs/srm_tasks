using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UILoad1 : MonoBehaviour
{
    public TMP_Dropdown startHoursDropdown;
    public TMP_Dropdown startMinutesDropdown;
    public TMP_Dropdown startSecondsDropdown;
    public TMP_Dropdown endHoursDropdown;
    public TMP_Dropdown endMinutesDropdown;
    public TMP_Dropdown endSecondsDropdown;
    public Button filterButton;
    public ApiRequest parkingSlotManager;
    public ParkingSlotBorderController[] parkingSlots; // Array of all parking slot border controllers

    // Vehicle prefabs
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
        // Initialize the vehicle prefabs dictionary
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

        InitializeDropdown(startHoursDropdown, GenerateNumericLabels(0, 23), "Hours");
        InitializeDropdown(startMinutesDropdown, GenerateNumericLabels(0, 59), "Minutes");
        InitializeDropdown(startSecondsDropdown, GenerateNumericLabels(0, 59), "Seconds");

        InitializeDropdown(endHoursDropdown, GenerateNumericLabels(0, 23), "Hours");
        InitializeDropdown(endMinutesDropdown, GenerateNumericLabels(0, 59), "Minutes");
        InitializeDropdown(endSecondsDropdown, GenerateNumericLabels(0, 59), "Seconds");

        if (filterButton != null)
        {
            filterButton.onClick.AddListener(OnFilterButtonClicked);
        }
        else
        {
            Debug.LogError("FilterButton is not assigned in the Inspector.");
        }
    }

    void InitializeDropdown(TMP_Dropdown dropdown, List<string> options, string label)
    {
        dropdown.ClearOptions();
        dropdown.options.Add(new TMP_Dropdown.OptionData(label));
        dropdown.AddOptions(options);
    }

    List<string> GenerateNumericLabels(int minValue, int maxValue)
    {
        List<string> labels = new List<string>();
        for (int i = minValue; i <= maxValue; i++)
        {
            labels.Add(i.ToString("D2"));
        }
        return labels;
    }

    void OnFilterButtonClicked()
    {
        string startTime = $"{startHoursDropdown.options[startHoursDropdown.value].text}:{startMinutesDropdown.options[startMinutesDropdown.value].text}:{startSecondsDropdown.options[startSecondsDropdown.value].text}";
        string endTime = $"{endHoursDropdown.options[endHoursDropdown.value].text}:{endMinutesDropdown.options[endMinutesDropdown.value].text}:{endSecondsDropdown.options[endSecondsDropdown.value].text}";
        Debug.Log($"Filter button clicked. Start time: {startTime}, End time: {endTime}");
        PopulateParkingSlots(startTime, endTime);
    }

    void PopulateParkingSlots(string startTime, string endTime)
    {
        List<ApiRequest.ParkingSlot> parkingSlotsData = parkingSlotManager.GetParkingSlots();

        if (parkingSlotsData == null)
        {
            Debug.LogError("Parking slots data is null.");
            return;
        }

        foreach (var slot in parkingSlots)
        {
            slot.SetAvailability(true); // Reset all slots to available
        }

        foreach (var slotData in parkingSlotsData)
        {
            string slotTime = slotData.timestamp.Split(' ')[1];
            if (string.Compare(slotTime, startTime) >= 0 && string.Compare(slotTime, endTime) <= 0)
            {
                foreach (var slot in parkingSlots)
                {
                    string slotNumber = slot.gameObject.name.Replace("Slot_", ""); // Assuming parking slot game object names are like "Slot_1", "Slot_2", etc.
                    if (slotNumber == slotData.slot_number.Trim())
                    {
                        string vehicleInfo = $"Name: {slotData.name}\nCar Model: {slotData.car_model}\nVehicle: {slotData.vehicle_number}";
                        slot.SetAvailability(false, vehicleInfo);

                        // Spawn the specific vehicle based on the car model
                        if (vehiclePrefabs.TryGetValue(slotData.car_model.ToLower(), out GameObject prefab))
                        {
                            slot.SpawnSpecificVehicle(prefab);
                        }
                        else
                        {
                            Debug.LogWarning($"No prefab found for car model: {slotData.car_model}");
                        }
                        break;
                    }
                }
            }
        }
    }
}