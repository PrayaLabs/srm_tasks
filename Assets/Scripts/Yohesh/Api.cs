using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Api : MonoBehaviour
{
    private string apiUrl = "https://prayalabs.com/rest/api/car-parking";

    // Class to match the structure of the JSON data
    [Serializable]
    public class ParkingSlot
    {
        public string timestamp;
        public string vehicle_number;
        public string car_model;
        public string in_time;
        public float parked_time;
        public string out_time;
        public int slot_number;
        public string apartment;
        public string name;
    }

    [Serializable]
    public class ParkingSlotList
    {
        public List<ParkingSlot> slots;
    }

    void Start()
    {
        StartCoroutine(GetParkingSlotData());
    }

    IEnumerator GetParkingSlotData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string json = webRequest.downloadHandler.text;

                string wrappedJson = "{\"slots\":" + json + "}";
                ParkingSlotList parkingSlotList = JsonUtility.FromJson<ParkingSlotList>(wrappedJson);

                foreach (var slot in parkingSlotList.slots)
                {
                    Debug.Log($"Slot {slot.slot_number} is occupied by {slot.name} with vehicle number {slot.vehicle_number}");
                    Debug.Log(slot.slot_number);
                }
            }
        }
    }
}