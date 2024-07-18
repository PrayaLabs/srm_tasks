using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UILoad : MonoBehaviour
{
    public TMP_Dropdown startHoursDropdown;
    public TMP_Dropdown startMinutesDropdown;
    public TMP_Dropdown startSecondsDropdown;
    public TMP_Dropdown endHoursDropdown;
    public TMP_Dropdown endMinutesDropdown;
    public TMP_Dropdown endSecondsDropdown;
    public TMP_Dropdown slotNumbersDropdown;
    public Button filterButton;
    public ApiRequest parkingSlotManager;

    void Start()
    {
        InitializeDropdown(startHoursDropdown, GenerateNumericLabels(0, 23), "Hours");
        InitializeDropdown(startMinutesDropdown, GenerateNumericLabels(0, 59), "Minutes");
        InitializeDropdown(startSecondsDropdown, GenerateNumericLabels(0, 59), "Seconds");

        InitializeDropdown(endHoursDropdown, GenerateNumericLabels(0, 23), "Hours");
        InitializeDropdown(endMinutesDropdown, GenerateNumericLabels(0, 59), "Minutes");
        InitializeDropdown(endSecondsDropdown, GenerateNumericLabels(0, 59), "Seconds");

        slotNumbersDropdown.ClearOptions();
        slotNumbersDropdown.options.Add(new TMP_Dropdown.OptionData("Slot Numbers"));

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
        PopulateSlotNumbersDropdown(startTime, endTime);
    }

    void PopulateSlotNumbersDropdown(string startTime, string endTime)
    {
        List<ApiRequest.ParkingSlot> parkingSlots = parkingSlotManager.GetParkingSlots();

        if (parkingSlots == null)
        {
            Debug.LogError("Parking slots data is null.");
            return;
        }

        List<string> slotNumbers = new List<string>();

        foreach (var slot in parkingSlots)
        {
            string slotTime = slot.timestamp.Split(' ')[1];
            if (string.Compare(slotTime, startTime) >= 0 && string.Compare(slotTime, endTime) <= 0)
            {
                slotNumbers.Add(slot.slot_number.ToString().Trim());
            }
        }

        if (slotNumbers.Count == 0)
        {
            Debug.Log("No slot numbers found for the given time range.");
            slotNumbersDropdown.ClearOptions();
            slotNumbersDropdown.options.Add(new TMP_Dropdown.OptionData("Slot Numbers"));
        }
        else
        {
            slotNumbersDropdown.ClearOptions();
            slotNumbersDropdown.AddOptions(slotNumbers);
            slotNumbersDropdown.onValueChanged.AddListener(OnSlotNumberSelected);
        }
    }

    void OnSlotNumberSelected(int index)
    {
        string selectedSlotNumber = slotNumbersDropdown.options[slotNumbersDropdown.value].text.Trim();
        Debug.Log($"Selected slot number: {selectedSlotNumber}");
        PrintSlotDetails(selectedSlotNumber);
    }

    void PrintSlotDetails(string slotNumber)
    {
        List<ApiRequest.ParkingSlot> parkingSlots = parkingSlotManager.GetParkingSlots();

        if (parkingSlots == null)
        {
            Debug.LogError("Parking slots data is null.");
            return;
        }

        int slotNumberInt;
        if (!int.TryParse(slotNumber, out slotNumberInt))
        {
            Debug.LogError("Invalid slot number format.");
            return;
        }

        foreach (var slot in parkingSlots)
        {
            if (slot.slot_number == null)
            {
                Debug.Log("slot.slot_number is null");
                continue;
            }

            Debug.Log($"Comparing '{slot.slot_number}' with '{slotNumberInt}'");

            if (slot.slot_number == slotNumberInt)
            {
                Debug.Log($"Name: {slot.name}");
                Debug.Log($"Apartment: {slot.apartment}");
                Debug.Log($"Vehicle Number: {slot.vehicle_number}");
                return;
            }
        }

        Debug.Log($"No details found for slot number: {slotNumber}");
    }
}



