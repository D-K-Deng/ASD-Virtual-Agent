using UnityEngine;
using System;
using BestHTTP.WebSocket;
using BestHTTP;
using System.IO;
using System.Collections;
using System.Diagnostics.Tracing;

public class WsClient : MonoBehaviour
{
    public static WsClient sharedWs = new WsClient();

    // Server configuration - local/internal only
    private string serverHost = "ifosa.org";
    private string port = "5080";
    private string wsrouter = "i/status";
    private string voiceRoute = "i/voice";
    private string picRoute = "i/picemo";
    private string appdata_path = "i/app";
    private string restart_path = "i/restart";

    private WebSocket ws_conn;

    public static string face_emo = null;
    public static string tempo_emo = null;
    public static string printed_text = null;

    public string board_text = "Connecting to server...";
    public string deviceId = SystemInfo.deviceUniqueIdentifier;
    public string ws_status = "System initializing...";

    private WsClient() { }

    public void NewWsConnect()
    {
        try
        {
            string ws_url = string.Format("ws://{0}:{1}/{2}", serverHost, port, wsrouter);
            ws_conn = new WebSocket(new Uri(ws_url));
            ws_conn.OnOpen += OnWebSocketOpen;
            ws_conn.OnMessage += OnWsMessageReceived;
            ws_conn.OnClosed += OnWebSocketClosed;
            ws_conn.OnError += OnWsError;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void OpenConnect()
    {
        ws_conn.Open();
    }

    public void CloseConnect()
    {
        ws_conn.Close();
    }

    // Send image data
    public void sendText_pic(string data)
    {
        string tmp = "{ \"uid\":\"" + deviceId + "\",\"data\":\"" + data + "\"}";
        postPicData(data);
        ws_status = "Analyzing image emotion...";
    }

    // Send voice data
    public void sendText_voice(string data, string method)
    {
        postVoiceData(data);
    }

    // Handle message received from server
    private void OnWsMessageReceived(WebSocket webSocket, string message)
    {
        Debug.Log("-WS Server:\n" + message);

        if (message == "Welcome aboard!")
        {
            ws_status = "Please ask a question";
            board_text = "Please ask a question";
        }

        if (message.Substring(0, 3) == "TTS")
        {
            ws_status = "Audio generated, playing response";
            PlaySound.instance.LoadMp3(message.Substring(4));
        }
        else if (message.Substring(0, 5) == "Voice")
        {
            ws_status = "Voice recognized, generating response...";
            board_text = message;
        }
        else if (message.Substring(0, 3) == "GPT")
        {
            ws_status = "Generating response, preparing audio...";
            board_text = message;
        }
        else if (message.Substring(0, 3) == "EMO")
        {
            Micro_Camera.updateStatus = "sdp";
            Debug.Log("Emotion based on image: " + message);
            face_emo = message.Substring(4);
        }
        else if (message.Substring(0, 4) == "TEMO")
        {
            Micro_Camera.updateStatus = "sdp";
            Debug.Log("Emotion based on text: " + message);
            tempo_emo = message.Substring(5);
        }
    }

    public static string GetMatchStr(string source)
    {
        string pattern = "[A-Za-z0-9\u4e00-\u9fa5-]+";
        string MatchStr = "";
        System.Text.RegularExpressions.MatchCollection results = System.Text.RegularExpressions.Regex.Matches(source, pattern);
        foreach (var s in results)
        {
            MatchStr += s.ToString();
        }
        return MatchStr;
    }

    // Called when connection is opened
    private void OnWebSocketOpen(WebSocket webSocket)
    {
        ws_status = "Connected to server";
        string tmp = "start:" + deviceId;
        ws_conn.Send(tmp);
        ws_status = "Sent user ID";
        Debug.Log("WebSocket is now Open!");
    }

    // Called when connection is closed
    private void OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)
    {
        Debug.Log("WebSocket is now Closed!");
    }

    // Handle connection error
    void OnWsError(WebSocket ws, string error)
    {
        ws_status = "Connection lost! Please restart.";
        Debug.Log("Error: " + error);
        GameObject.Find("NetworkAlert_voice").GetComponent<AudioSource>().Play();
    }

    // Send application data
    public void sendPostData(string data)
    {
        string tmpUrl = string.Format("http://{0}:{1}/{2}", serverHost, port, appdata_path);
        HTTPRequest request = new HTTPRequest(new Uri(tmpUrl), HTTPMethods.Post, OnRequestFinished);
        request.RawData = System.Text.Encoding.UTF8.GetBytes(data);
        request.Send();
    }

    void OnRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        // Debug.Log("App response: " + response.Data);
    }

    // Send restart signal
    public void sendRestart()
    {
        string tmpUrl = string.Format("http://{0}:{1}/{2}", serverHost, port, restart_path);
        HTTPRequest request = new HTTPRequest(new Uri(tmpUrl), HTTPMethods.Post);
        string tmp = "{ \"uid\":\"" + deviceId + "\",\"data\":\"" + deviceId + "\"}";
        Debug.Log(tmp);
        request.RawData = System.Text.Encoding.UTF8.GetBytes(tmp);
        request.Send();
        Debug.Log("Voice restart sent!");
        ws_status = "New session started";
    }

    // Post voice data to server
    public void postVoiceData(string data)
    {
        string tmpUrl = string.Format("http://{0}:{1}/{2}", serverHost, port, voiceRoute);
        HTTPRequest request = new HTTPRequest(new Uri(tmpUrl), HTTPMethods.Post);
        string tmp = "{ \"uid\":\"" + deviceId + "\",\"data\":\"" + data + "\"}";
        request.RawData = System.Text.Encoding.UTF8.GetBytes(tmp);
        request.Send();
    }

    // Post image data to server
    public void postPicData(string data)
    {
        string tmpUrl = string.Format("http://{0}:{1}/{2}", serverHost, port, picRoute);
        HTTPRequest request = new HTTPRequest(new Uri(tmpUrl), HTTPMethods.Post);
        string tmp = "{ \"uid\":\"" + deviceId + "\",\"data\":\"" + data + "\"}";
        request.RawData = System.Text.Encoding.UTF8.GetBytes(tmp);
        request.Send();
        Debug.Log("Facial recognition image sent");
    }
}
