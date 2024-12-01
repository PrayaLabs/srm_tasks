using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;

public class UILoad1 : MonoBehaviour
{
    // Dropdown UI elements
    public TMP_Dropdown startHoursDropdown;
    public TMP_Dropdown startMinutesDropdown;
    public TMP_Dropdown startSecondsDropdown;
    public TMP_Dropdown endHoursDropdown;
    public TMP_Dropdown endMinutesDropdown;
    public TMP_Dropdown endSecondsDropdown;

    // Buttons
    public Button filterButton;
    public Button menuButton;
    public Button exitButton;
    public Button searchButton;
    public Button detailsButton;
    public Button SceneButton;

    // Search input field
    public TMP_InputField searchInputField;

    // UI Containers
    public GameObject searchInputUI;
    public GameObject mainUIContainer;
    public GameObject freeSlotsPanel;
    public GameObject lockedSlotsPanel;

    // Panels for slot details
    public Transform freeSlotsContent;
    public Transform lockedSlotsContent;
    public GameObject slotItemPrefab;

    // Parking Slots and API Manager
    public ApiRequest parkingSlotManager;
    public ParkingSlotBorderController[] parkingSlots;
    public GameObject detailField1;
    public GameObject detailField2;

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

    // Variable to store search result
    private ParkingSlotData selectedSlot;

    void Start()
    {
        InitializeVehiclePrefabs();
        InitializeDropdowns();
        AssignButtonListeners();

        // Set initial visibility
        ToggleUIVisibility(mainUIContainer, false);
        ToggleUIVisibility(searchInputUI, false);
        ToggleUIVisibility(freeSlotsPanel, false);
        ToggleUIVisibility(lockedSlotsPanel, false);

        // Use a coroutine to show currently parked slots on start
        StartCoroutine(WaitAndShowParkedSlots());
    }



    private IEnumerator WaitAndShowParkedSlots()
    {
        // Ensure parkingSlotManager is initialized
        if (parkingSlotManager == null)
        {
            Debug.LogError("Parking Slot Manager is not initialized.");
            yield break;
        }

        Debug.Log("Waiting for parking slot data to be ready...");

        // Wait for the data to be initialized (replace this condition with actual readiness check if needed)
        while (parkingSlotManager.GetParkingSlots() == null || !parkingSlotManager.GetParkingSlots().Any())
        {
            yield return new WaitForSeconds(0.5f); // Check every 0.5 seconds
        }

        Debug.Log("Parking slot data is ready. Proceeding to show parked slots.");

        ShowParkedSlotsOnStart();
    }

    private void ShowParkedSlotsOnStart()
    {
        // Ensure parkingSlotManager is not null
        if (parkingSlotManager == null)
        {
            Debug.LogError("Parking Slot Manager is not initialized.");
            return;
        }

        // Get current time and 10 minutes later
        DateTime now = DateTime.Now;
        DateTime tenMinutesLater = now.AddMinutes(10);

        string currentTime = now.ToString("HH:mm:ss");
        string futureTime = tenMinutesLater.ToString("HH:mm:ss");

        Debug.Log($"Current Time: {currentTime}, Future Time: {futureTime}");

        // Fetch parking slot data
        var parkingSlotsData = parkingSlotManager.GetParkingSlots();

        // Log and check if the data is null or empty
        if (parkingSlotsData == null || !parkingSlotsData.Any())
        {
            Debug.LogError("No parking slots data available or empty list returned.");
            return;
        }

        Debug.Log($"Total Parking Slots Retrieved: {parkingSlotsData.Count()}");

        // Reset slot availability
        foreach (var slot in parkingSlots)
        {
            slot.SetAvailability(true, null);
        }

        // Update based on time interval
        foreach (var slotData in parkingSlotsData)
        {
            string slotTime = slotData.timestamp.Split(' ')[1].Trim(); // Extract time
            //Debug.Log($"Slot Data Timestamp: {slotData.timestamp}, Extracted Time: {slotTime}");

            // Compare times
            if (string.Compare(slotTime, currentTime) >= 0 && string.Compare(slotTime, futureTime) <= 0)
            {
                foreach (var slot in parkingSlots)
                {
                    if (slot.gameObject.name.Replace("Slot_", "") == slotData.slot_number.Trim())
                    {
                        //Debug.Log($"Slot {slotData.slot_number} is occupied by {slotData.name}");
                        slot.SetAvailability(false, $"Name: {slotData.name}\nCar Model: {slotData.car_model}\nVehicle: {slotData.vehicle_number}");

                        if (vehiclePrefabs.TryGetValue(slotData.car_model.ToLower(), out GameObject prefab))
                        {
                            slot.SpawnSpecificVehicle(prefab);
                        }
                        break;
                    }
                }
            }
        }

        // Update UI panels
        ShowDetailsPanels();

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

    private void InitializeDropdowns()
    {
        // Initialize time dropdowns with options
        InitializeDropdown(startHoursDropdown, GenerateNumericLabels(0, 23), "Hours");
        InitializeDropdown(startMinutesDropdown, GenerateNumericLabels(0, 59), "Minutes");
        InitializeDropdown(startSecondsDropdown, GenerateNumericLabels(0, 59), "Seconds");
        InitializeDropdown(endHoursDropdown, GenerateNumericLabels(0, 23), "Hours");
        InitializeDropdown(endMinutesDropdown, GenerateNumericLabels(0, 59), "Minutes");
        InitializeDropdown(endSecondsDropdown, GenerateNumericLabels(0, 59), "Seconds");
    }

    private void AssignButtonListeners()
    {
        if (filterButton != null) filterButton.onClick.AddListener(OnFilterButtonClicked);
        if (menuButton != null) menuButton.onClick.AddListener(() => ToggleUIVisibility(mainUIContainer, !mainUIContainer.activeSelf));
        if (exitButton != null) exitButton.onClick.AddListener(() => Application.Quit());
        if (searchButton != null) searchButton.onClick.AddListener(() =>
        {
            bool isActive = searchInputUI.activeSelf;
            ToggleUIVisibility(searchInputUI, !isActive);
            ToggleUIVisibility(SceneButton.gameObject, !isActive); // Add another button toggle
        });
        if (SceneButton != null) SceneButton.onClick.AddListener(OnSceneButtonClicked);
        if (detailsButton != null) detailsButton.onClick.AddListener(() =>
        {
            bool isActive = freeSlotsPanel.activeSelf;
            ToggleUIVisibility(freeSlotsPanel, !isActive);
            ToggleUIVisibility(lockedSlotsPanel, !isActive);
            ToggleUIVisibility(detailField1, !isActive);
            ToggleUIVisibility(detailField2, !isActive);
            if (!isActive)
            {
                ShowDetailsPanels();
            }
        });
    }

    private void InitializeDropdown(TMP_Dropdown dropdown, List<string> options, string label)
    {
        if (dropdown != null)
        {
            dropdown.ClearOptions();
            dropdown.options.Add(new TMP_Dropdown.OptionData(label));
            dropdown.AddOptions(options);
        }
    }

    private List<string> GenerateNumericLabels(int minValue, int maxValue)
    {
        var labels = new List<string>();
        for (int i = minValue; i <= maxValue; i++)
        {
            labels.Add(i.ToString("D2"));
        }
        return labels;
    }

    private void ToggleUIVisibility(GameObject uiElement, bool isVisible)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(isVisible);
        }
    }

    private void OnFilterButtonClicked()
    {
        string startTime = GetSelectedTime(startHoursDropdown, startMinutesDropdown, startSecondsDropdown);
        string endTime = GetSelectedTime(endHoursDropdown, endMinutesDropdown, endSecondsDropdown);

        // Clear and update the slot availability
        PopulateParkingSlots(startTime, endTime);

        // After populating the parking slots, update the panels
        ShowDetailsPanels();
    }

    private string GetSelectedTime(TMP_Dropdown hours, TMP_Dropdown minutes, TMP_Dropdown seconds)
    {
        return $"{hours.options[hours.value].text}:{minutes.options[minutes.value].text}:{seconds.options[seconds.value].text}";
    }

    private void PopulateParkingSlots(string startTime, string endTime)
    {
        var parkingSlotsData = parkingSlotManager.GetParkingSlots();

        if (parkingSlotsData == null)
        {
            Debug.LogError("Parking slots data is null.");
            return;
        }

        foreach (var slot in parkingSlots)
        {
            slot.SetAvailability(true, null);
        }

        foreach (var slotData in parkingSlotsData)
        {
            string slotTime = slotData.timestamp.Split(' ')[1];

            if (string.Compare(slotTime, startTime) >= 0 && string.Compare(slotTime, endTime) <= 0)
            {
                foreach (var slot in parkingSlots)
                {
                    if (slot.gameObject.name.Replace("Slot_", "") == slotData.slot_number.Trim())
                    {
                        slot.SetAvailability(false, $"Name: {slotData.name}\nCar Model: {slotData.car_model}\nVehicle: {slotData.vehicle_number}");

                        if (vehiclePrefabs.TryGetValue(slotData.car_model.ToLower(), out GameObject prefab))
                        {
                            slot.SpawnSpecificVehicle(prefab);
                        }
                        break;
                    }
                }
            }
        }
    }

    private void ShowDetailsPanels()
    {
        ClearPanelContent(freeSlotsContent);
        ClearPanelContent(lockedSlotsContent);

        foreach (var slot in parkingSlots)
        {
            var targetContent = slot.IsAvailable ? freeSlotsContent : lockedSlotsContent;
            AddSlotToPanel(targetContent, slot.gameObject.name);
        }


    }

    private void AddSlotToPanel(Transform content, string slotName)
    {
        if (content == null || slotItemPrefab == null) return;

        GameObject slotItem = Instantiate(slotItemPrefab, content);
        var textComponent = slotItem.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = slotName;
        }
    }

    private void ClearPanelContent(Transform content)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
    private void OnSceneButtonClicked()
    {
        // Retrieve vehicle number from the input field
        string vehicleNumber = searchInputField.text.Trim();

        if (string.IsNullOrEmpty(vehicleNumber))
        {
            Debug.Log("Please enter a vehicle number.");
            return;
        }

        // Check if we are using initial real-time values or dropdown values
        string startTime, endTime;
        if (startHoursDropdown.value == 0 && startMinutesDropdown.value == 0 && startSecondsDropdown.value == 0 &&
            endHoursDropdown.value == 0 && endMinutesDropdown.value == 0 && endSecondsDropdown.value == 0)
        {
            // Use real-time logic
            DateTime now = DateTime.Now;
            DateTime tenMinutesLater = now.AddMinutes(10);
            startTime = now.ToString("HH:mm:ss");
            endTime = tenMinutesLater.ToString("HH:mm:ss");
            Debug.Log("Using real-time update for SceneButton functionality.");
        }
        else
        {
            // Use dropdown-selected values
            startTime = GetSelectedTime(startHoursDropdown, startMinutesDropdown, startSecondsDropdown);
            endTime = GetSelectedTime(endHoursDropdown, endMinutesDropdown, endSecondsDropdown);
            Debug.Log("Using dropdown values for SceneButton functionality.");
        }

        // Fetch parking slot data
        var parkingSlotsData = parkingSlotManager.GetParkingSlots();

        if (parkingSlotsData == null)
        {
            Debug.LogError("No parking slots data available.");
            return;
        }

        ParkingSlotBorderController foundSlot = null;
        ParkingSlotData selectedSlotData = null;

        foreach (var slot in parkingSlots)
        {
            foreach (var slotData in parkingSlotsData)
            {
                if (slotData.slot_number == slot.gameObject.name.Replace("Slot_", ""))
                {
                    string slotTime = slotData.timestamp.Split(' ')[1]; // Extract time (HH:MM:SS)

                    if (string.Compare(slotTime, startTime) >= 0 && string.Compare(slotTime, endTime) <= 0 &&
                        slotData.vehicle_number == vehicleNumber)
                    {
                        Debug.Log($"Vehicle {vehicleNumber} found in slot {slot.gameObject.name}.");
                        foundSlot = slot;

                        selectedSlotData = new ParkingSlotData
                        {
                            Name = slotData.name,
                            CarModel = slotData.car_model,
                            VehicleNumber = slotData.vehicle_number,
                            SlotNumber = slotData.slot_number,
                        };

                        break;
                    }
                }
            }
        }

        if (foundSlot != null)
        {
            Debug.Log($"Slot {selectedSlotData.SlotNumber} selected for vehicle {selectedSlotData.VehicleNumber}.");

            // Save the selected slot data to PlayerPrefs
            string jsonSlotData = JsonUtility.ToJson(selectedSlotData);
            PlayerPrefs.SetString("SelectedSlot", jsonSlotData);
            PlayerPrefs.Save();

            // Load CarDetails scene
            SceneManager.LoadScene("CarDetails");
        }
        else
        {
            Debug.Log($"Vehicle {vehicleNumber} not found in any slot during the selected time range.");
        }
    }




}
