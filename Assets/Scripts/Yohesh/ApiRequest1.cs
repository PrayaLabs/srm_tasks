using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ApiRequest: MonoBehaviour
{
    private string apiUrl = "https://prayalabs.com/rest/api/car-parking";

    [Serializable]
    public class ParkingSlot
    {
        public string timestamp;
        public string vehicle_number;
        public string car_model;
        public string in_time;
        public float parked_time;
        public string out_time;
        public string slot_number;
        public string apartment;
        public string name;
    }

    [Serializable]
    public class ParkingSlotList
    {
        public List<ParkingSlot> slots;
    }

    private List<ParkingSlot> slot;

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
                slot = parkingSlotList.slots;
                
            }
        }
    }

    public List<ParkingSlot> GetParkingSlots()
    {
        return slot;
    }
}

