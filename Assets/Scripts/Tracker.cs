using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Tracker : MonoBehaviour
{

    private static Tracker instance;

    public static Tracker Instance
    {
        get
        {
            return instance;
        }
    }
    private string deviceID;
    private static Dictionary<string, List<string>> activities;
    private static List<string> appSessions;

    private const float sendInterval = 10.0f; //10 seconds per send
    private Coroutine automaticDataSendRoutineCoroutine = null;


    void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Debug.Log("Scene opened: " + scene.name);
            string Name = scene.name;
            RecordActivityStart(Name);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            //Debug.Log("Scene closed: "+ scene.name);
            string Name = scene.name;
            RecordActivityEnd(Name);
        }



    public void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        deviceID = SystemInfo.deviceUniqueIdentifier;
        activities = new Dictionary<string, List<string>>();
        appSessions = new List<string>();

        appSessions.Add(TimeStamp() + "|start");
    }

    public void Start()
    {
        automaticDataSendRoutineCoroutine = StartCoroutine(AutomaticDataSendRoutine());
    }

    private bool hasSentDataInBackground = false;

    public void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            appSessions.Add(TimeStamp() + "|pause");
            if (!hasSentDataInBackground)
            {
                hasSentDataInBackground = true;
                SaveData();
            }
        }
        else
        {
            appSessions.Add(TimeStamp() + "|resume");
            hasSentDataInBackground = false;
        }
    }

    public void OnApplicationQuit()
    {// This ensures data is saved when the application is closed.
        appSessions.Add(TimeStamp() + "|quit");

        foreach (var activity in activities)
        {
        List<string> records = activity.Value;
        if (records.Count > 0 && !records[records.Count -1].Contains("|end"))
        {
            records.Add(TimeStamp() + "|end");
        }
        }


        if (automaticDataSendRoutineCoroutine != null)
        {
            StopCoroutine(automaticDataSendRoutineCoroutine);
        }
        SaveData();
        //SaveDataLocally(); // run this if you want to save data locally and debug
    }
    private static string GetCurrentSceneName()
    {// This gets the current scene name
        return SceneManager.GetActiveScene().name;
    }

    private static string GetButtonName()
    {// This gets the current button name
        return EventSystem.current.currentSelectedGameObject.name;
    }

    private static GameObject GetQuestionButton()
    {
        return EventSystem.current.currentSelectedGameObject;
    }

    public static void TrackButtonClick()
    {// Call this function when a button is clicked
        string buttonName = "Click_" + GetCurrentSceneName() + "_" + GetButtonName();
        //Debug.Log(buttonName);
        //RecordActivity(buttonName); //deprecated
    }

    public static void TrackButtonEnd()
    {
        string buttonName = "End_" + GetCurrentSceneName() + "_" + GetButtonName();
        RecordActivity(buttonName);
    }
    
    public static void TrackQuestionText()
    {
        GameObject quesButton = GetQuestionButton();
        Text textComponent = quesButton.GetComponent<Text>();
        RecordQuestion(GetCurrentSceneName(), textComponent.text);
        //RecordActivity(textComponent.text);
        //Debug.Log("The question: " + textComponent.text);
    }

    private static void RecordActivity(string Name)
    {// This records a button click
        if (!activities.ContainsKey(Name))
        {
            activities[Name] = new List<string>();
        }
        activities[Name].Add(TimeStamp());
    }
        private static void RecordQuestion(string Name, string text)
    {// This records a button click
        if (!activities.ContainsKey(Name))
        {
            activities[Name] = new List<string>();
        }
        activities[Name].Add(TimeStamp()+"_"+text);
    }

        private static void RecordActivityStart(string Name)
    {// This records a button click
        if (!activities.ContainsKey(Name))
        {
            activities[Name] = new List<string>();
        }
        activities[Name].Add(TimeStamp()+"|start");
    }

        private static void RecordActivityEnd(string Name)
    {// This records a button click
        if (!activities.ContainsKey(Name))
        {
            activities[Name] = new List<string>();
        }
        activities[Name].Add(TimeStamp()+"|end");
    }

    private IEnumerator AutomaticDataSendRoutine()// This sends data automatically every pre-set seconds
    {
        Debug.Log("AutomaticDataSendRoutine called");
        while (true)
        {
            yield return new WaitForSeconds(sendInterval);
            //StartCoroutine(SaveData());
            SaveData();
        }
    }

    private static string TimeStamp()
    {// This gets the current time stamp
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }


    private void SaveData()// This saves data to the server
    {
        var data = new
        {
            deviceID = deviceID,
            appSessions = appSessions,
            activities =   activities

        };
        // Convert to JSON string
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        Debug.Log(jsonData);
        WsClient.sharedWs.sendPostData(jsonData);
    }

    private void SaveDataLocally()
    {
        var data = new
        {
            deviceID = deviceID,
            appSessions = appSessions,
            activities = activities
        };

        // Convert to JSON string
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Define the file path, change this to your own if needed
        string desktopPath = "C:\\Users\\28621\\Desktop";
        string filePath = Path.Combine(desktopPath, "TrackerData.txt");

        // Write the JSON data to a file on the desktop
        File.WriteAllText(filePath, jsonData);
    }



}
