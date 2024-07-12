using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Dropdown hoursDropdown;
    public TMP_Dropdown minutesDropdown;
    public TMP_Dropdown secondsDropdown;
    public Button filterButton;
    public ApiRequest parkingSlotManager;

    void Start()
    {
        // Initialize dropdown options with labels
        InitializeDropdown(hoursDropdown, GenerateNumericLabels(0, 23), "Hours");
        InitializeDropdown(minutesDropdown, GenerateNumericLabels(0, 59), "Minutes");
        InitializeDropdown(secondsDropdown, GenerateNumericLabels(0, 59), "Seconds");

        if (filterButton != null)
        {
            filterButton.onClick.AddListener(OnFilterButtonClicked);
        }
        else
        {
            Debug.LogError("FilterButton is not assigned in the Inspector.");
        }

        /*if (hoursDropdown == null)
        {
            Debug.LogError("HoursDropdown is not assigned in the Inspector.");
        }

        if (minutesDropdown == null)
        {
            Debug.LogError("MinutesDropdown is not assigned in the Inspector.");
        }

        if (secondsDropdown == null)
        {
            Debug.LogError("SecondsDropdown is not assigned in the Inspector.");
        }

        if (parkingSlotManager == null)
        {
            Debug.LogError("ParkingSlotManager is not assigned in the Inspector.");
        }*/
    }

    void InitializeDropdown(TMP_Dropdown dropdown, List<string> options, string label)
    {
        dropdown.ClearOptions(); // Clear existing options

        dropdown.options.Add(new TMP_Dropdown.OptionData(label));
        dropdown.AddOptions(options);
    }

    List<string> GenerateNumericLabels(int minValue, int maxValue)
    {
        List<string> labels = new List<string>();
        for (int i = minValue; i <= maxValue; i++)
        {
            labels.Add(i.ToString("D2")); // Format as two digits
        }
        return labels;
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

        bool slotFound = false;
        foreach (var slot in parkingSlots)
        {
            string slotTime = slot.timestamp.Split(' ')[1];
            if (slotTime.Equals(time))
            {
                slotFound = true;
                Debug.Log($"Vehicle Number: {slot.vehicle_number}");
                Debug.Log($"Car Model: {slot.car_model}");
                Debug.Log($"In Time: {slot.in_time}");
                Debug.Log($"Parked Time: {slot.parked_time}");
                Debug.Log($"Out Time: {slot.out_time}");
                Debug.Log($"Slot Number: {slot.slot_number}");
                Debug.Log($"Apartment: {slot.apartment}");
                Debug.Log($"Name: {slot.name}");
            }
        }

        if (!slotFound)
        {
            Debug.Log($"No slots found for time: {time}");
        }
    }
}
