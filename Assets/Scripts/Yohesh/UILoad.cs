using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UILoad : MonoBehaviour
{
    public TMP_Dropdown hoursDropdown;
    public TMP_Dropdown minutesDropdown;
    public TMP_Dropdown secondsDropdown;
    public TMP_Dropdown detailsDropdown;
    public Button filterButton;
    public ApiRequest parkingSlotManager;

    private List<ApiRequest.ParkingSlot> filteredSlots;

    void Start()
    {
        // Initialize dropdown options
        InitializeDropdown(hoursDropdown, 0, 23);
        InitializeDropdown(minutesDropdown, 0, 59);
        InitializeDropdown(secondsDropdown, 0, 59);

        if (filterButton != null)
        {
            filterButton.onClick.AddListener(OnFilterButtonClicked);
        }
        else
        {
            Debug.LogError("FilterButton is not assigned in the Inspector.");
        }

        if (detailsDropdown != null)
        {
            detailsDropdown.onValueChanged.AddListener(OnDetailsDropdownChanged);
        }
        else
        {
            Debug.LogError("DetailsDropdown is not assigned in the Inspector.");
        }
    }

    void InitializeDropdown(TMP_Dropdown dropdown, int minValue, int maxValue)
    {
        dropdown.ClearOptions(); // Clear existing options

        List<string> options = new List<string>();
        for (int i = minValue; i <= maxValue; i++)
        {
            options.Add(i.ToString("D2")); // Format as two digits
        }
        dropdown.AddOptions(options);
    }

    void OnFilterButtonClicked()
    {
        string selectedTime = $"{hoursDropdown.options[hoursDropdown.value].text}:{minutesDropdown.options[minutesDropdown.value].text}:{secondsDropdown.options[secondsDropdown.value].text}";
        Debug.Log($"Filter button clicked. Selected time: {selectedTime}");
        FilterDataByTime(selectedTime);
    }

    void FilterDataByTime(string time)
    {
        List<ApiRequest.ParkingSlot> parkingSlots = parkingSlotManager.GetParkingSlots();

        if (parkingSlots == null)
        {
            Debug.LogError("Parking slots data is null.");
            return;
        }

        filteredSlots = new List<ApiRequest.ParkingSlot>();
        foreach (var slot in parkingSlots)
        {
            string slotTime = slot.timestamp.Split(' ')[1];
            if (slotTime.Equals(time))
            {
                filteredSlots.Add(slot);
            }
        }

        if (filteredSlots.Count > 0)
        {
            PopulateDetailsDropdown(filteredSlots);
        }
        else
        {
            Debug.Log($"No slots found for time: {time}");
            detailsDropdown.ClearOptions();
        }
    }

    void PopulateDetailsDropdown(List<ApiRequest.ParkingSlot> slots)
    {
        detailsDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (var slot in slots)
        {
            options.Add(slot.name); // Populate with names; you can choose other details if needed
        }
        detailsDropdown.AddOptions(options);

        if (options.Count > 0)
        {
            detailsDropdown.value = 0; // Select the first item by default
            OnDetailsDropdownChanged(0); // Trigger the display of the first item's details
        }
    }

    void OnDetailsDropdownChanged(int index)
    {
        if (filteredSlots == null || index >= filteredSlots.Count)
        {
            Debug.LogError("Invalid selection in details dropdown.");
            return;
        }

        var selectedSlot = filteredSlots[index];
        Debug.Log($"Selected Slot Details:\n" +
                  $"Vehicle Number: {selectedSlot.vehicle_number}\n" +
                  $"Car Model: {selectedSlot.car_model}\n" +
                  $"In Time: {selectedSlot.in_time}\n" +
                  $"Parked Time: {selectedSlot.parked_time}\n" +
                  $"Out Time: {selectedSlot.out_time}\n" +
                  $"Slot Number: {selectedSlot.slot_number}\n" +
                  $"Apartment: {selectedSlot.apartment}\n" +
                  $"Name: {selectedSlot.name}");
    }
}
