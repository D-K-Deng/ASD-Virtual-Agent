using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class Log_Manager
{
    static JObject data = new JObject();

    static Log_Manager()
    {
        // Static constructor (optional logic can be placed here)
    }

    // Send the collected data to the server
    public static void send()
    {
        string jsonString = JsonConvert.SerializeObject(data);
        // Debug.Log(jsonString);
        Debug.Log("Sending data to server...");
        WsClient.sharedWs.sendPostData(jsonString);
    }

    // Add key-value pair to the data object
    // You can call this method from anywhere in the code
    public static void addData(string key, JObject value)
    {
        data[key] = value;
    }

    // Clear all collected data
    public static void clearData()
    {
        data.RemoveAll();
    }
}
