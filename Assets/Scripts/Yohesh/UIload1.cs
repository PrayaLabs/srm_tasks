using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        string vehicleNumber = searchInputField.text.Trim(); 

        if (string.IsNullOrEmpty(vehicleNumber))
        {
            Debug.Log("Please enter a vehicle number.");
            return;
        }

        // Get the selected time range from the dropdowns
        string startTime = GetSelectedTime(startHoursDropdown, startMinutesDropdown, startSecondsDropdown);
        string endTime = GetSelectedTime(endHoursDropdown, endMinutesDropdown, endSecondsDropdown);

        
        var parkingSlotsData = parkingSlotManager.GetParkingSlots();

        if (parkingSlotsData == null)
        {
            Debug.LogError("No parking slots data available.");
            return;
        }

        ParkingSlotBorderController foundSlot = null;
        bool isLocked = false;
        ParkingSlotData selectedSlotData = null; 

        foreach (var slot in parkingSlots)
        {
            Debug.Log($"Checking Slot: {slot.gameObject.name}");

            foreach (var slotData in parkingSlotsData)
            {
                
                if (slotData.slot_number == slot.gameObject.name.Replace("Slot_", ""))
                {
                    Debug.Log($"Slot {slot.gameObject.name} found in data, checking availability.");

                    
                    if (!slot.IsAvailable && slotData.vehicle_number == vehicleNumber)
                    {
                        string slotTime = slotData.timestamp.Split(' ')[1]; // Extract time (HH:MM:SS)

                        
                        if (string.Compare(slotTime, startTime) >= 0 && string.Compare(slotTime, endTime) <= 0)
                        {
                            Debug.Log($"Vehicle {vehicleNumber} found in locked slot {slot.gameObject.name} within the time range.");
                            foundSlot = slot;
                            isLocked = true;


                            selectedSlotData = new ParkingSlotData
                            {
                                Name = slotData.name,             
                                CarModel = slotData.car_model,         
                                VehicleNumber = slotData.vehicle_number, 
                                SlotNumber = slotData.slot_number,   
                            };

                            break; // Exit inner loop once a match is found
                        }
                    }
                }
            }

            
            if (isLocked) break;
        }

        // Proceed to CarDetails if a locked slot with the vehicle number is found and within the time range
        if (isLocked && selectedSlotData != null)
        {
            Debug.Log("Vehicle found in locked slot within time range. Proceeding to CarDetails.");

            string jsonSlotData = JsonUtility.ToJson(selectedSlotData);
            PlayerPrefs.SetString("SelectedSlot", jsonSlotData);

            // Load the CarDetails scene
            SceneManager.LoadScene("CarDetails");
        }
        else
        {
            Debug.Log("Vehicle not found in locked slots within the selected time range.");
        }
    }




}
