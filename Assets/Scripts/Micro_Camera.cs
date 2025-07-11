using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;
using UnityEngine.UI;

public class Micro_Camera : MonoBehaviour
{
    AudioClip myAudioClip;

    private bool camAvailable;
    private WebCamTexture frontCam;
    public static Color[] GetPixels;
    public static byte[] bytes;
    public static byte[] pre_bytes;
    public static string file;
    public static string fileAll;
    public static int iterateTimes = 1;
    public static byte[] result;
    public static string myString;
    public static bool sending = false;
    private Animator animator;
    public GameObject teacher;

    bool sendingPicControl = false;
    int pic_size_send = 400;
    int pic_box_size;

    public static bool start_asking_ques = false;

    public GameObject sounder;

    void Start()
    {
        animator = teacher.GetComponent<Animator>();
        SetupCamera();
        DeleteTempFiles();
    }

    Text wsStatus;
    Text boardText;

    private void Awake()
    {
        // Get UI text components via tag
        var tmp = GameObject.FindWithTag("WsStatus");
        wsStatus = tmp.GetComponent<Text>();

        tmp = GameObject.FindWithTag("BoardText");
        boardText = tmp.GetComponent<Text>();
    }

    void DeleteTempFiles()
    {
        string folderPath = Application.persistentDataPath + "/file_tmp/";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string[] files = Directory.GetFiles(folderPath);
        foreach (string file in files)
        {
            File.Delete(file);
        }

        Debug.Log("file_tmp folder cleared");
    }

    void FixedUpdate()
    {
        // Reserved for future frame-dependent logic
    }

    private void SetupCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                frontCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
                break;
            }
        }

        if (frontCam == null)
        {
            Debug.Log("Unable to find front camera");
            return;
        }

        Debug.Log("Device Name: " + frontCam.deviceName);
        frontCam.Play();
        camAvailable = true;

        WsClient.sharedWs.NewWsConnect();
        WsClient.sharedWs.OpenConnect();
    }

    public void StartAll()
    {
        // Stop any ongoing audio playback
        PlaySound.instance.StopMusic();

        Debug.Log("ACQ Start");

        myAudioClip = Microphone.Start(null, false, 60, 16000);
        WsClient.sharedWs.ws_status = "Speaking...";
        WsClient.sharedWs.board_text = "Speaking...";
        sendingPicControl = true;
        updateStatus = "sdp"; // 'sdp' = send picture

        start_asking_ques = true;
        PlaySound.finished_speaking = false;
    }

    public static string updateStatus;

    private void Update()
    {
        // Send picture only when in 'sdp' state and previous picture has been responded
        if (sendingPicControl)
        {
            switch (updateStatus)
            {
                case "sdp":
                    SendPic2Server();
                    updateStatus = "rec";
                    break;
                case "rec":
                    break;
            }
        }

        // UI updates
        wsStatus.text = WsClient.sharedWs.ws_status;
        boardText.text = WsClient.sharedWs.board_text;
    }

    public void StopALL()
    {
        Debug.Log("ACQ End");

        WsClient.sharedWs.ws_status = "Processing audio...";
        WsClient.sharedWs.board_text = "Processing audio...";
        sendingPicControl = false;

        int lastTime = Microphone.GetPosition(null);
        if (lastTime == 0)
        {
            return;
        }

        Microphone.End(null);

        float[] Samples = new float[myAudioClip.samples];
        myAudioClip.GetData(Samples, 0);
        float[] ClipSamples = new float[lastTime];
        Array.Copy(Samples, ClipSamples, ClipSamples.Length - 1);

        myAudioClip = AudioClip.Create("playRecordClip", ClipSamples.Length, 1, 16000, false);
        myAudioClip.SetData(ClipSamples, 0);

        StartCoroutine(SendVoice2Server(SavWav.GetAudioBase64Str(myAudioClip)));

        start_asking_ques = false;
    }

    public byte[] ConvertAudioClipToBytes(AudioClip audioClip)
    {
        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];

        int rescaleFactor = 32767;
        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        return bytesData;
    }

    void SendPic2Server()
    {
        var tmp = GetCurrentImage();
        Debug.Log("--send pic");
        WsClient.sharedWs.sendText_pic(tmp);
    }

    IEnumerator SendVoice2Server(string voiceBase64)
    {
        WsClient.sharedWs.sendText_voice(voiceBase64, "gpt");
        Debug.Log("--send voice data");
        yield return 0;
    }

    private string GetCurrentImage()
    {
        Texture2D pre_tex = new Texture2D(frontCam.width, frontCam.height);
        pic_box_size = (frontCam.width > frontCam.height) ? frontCam.height : frontCam.width;
        pre_tex.SetPixels(frontCam.GetPixels(0, 0, frontCam.width, frontCam.height));
        pre_tex.Apply();

        if (frontCam.deviceName == "前置相机")
        {
            pre_tex = rotate90(pre_tex);
        }

        Texture2D tex = ScaleTexture(pre_tex, pic_size_send, pic_size_send);
        bytes = tex.EncodeToJPG();
        pre_bytes = pre_tex.EncodeToJPG();
        string base64 = Convert.ToBase64String(bytes);
        var savetime = GetTimeStamp();

        File.WriteAllBytes(Application.persistentDataPath + "/file_tmp/pic" + savetime + ".jpg", bytes);
        File.WriteAllBytes(Application.persistentDataPath + "/file_tmp/pic" + savetime + "_pre.jpg", pre_bytes);
        File.WriteAllText(Application.persistentDataPath + "/file_tmp/pic" + savetime + ".txt", base64);

        return base64;
    }

    public static Texture2D ScaleTexture(Texture2D source, float targetWidth, float targetHeight)
    {
        Texture2D result = new Texture2D((int)targetWidth, (int)targetHeight, source.format, false);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / result.width, (float)i / result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }

    public Texture2D resize(Texture2D raw, int size)
    {
        Texture2D rtnTex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        float rescaleFactor = raw.width / size;

        for (int i = 0; i < rtnTex.height; i++)
        {
            for (int j = 0; j < rtnTex.width; j++)
            {
                rtnTex.SetPixel(j, i, raw.GetPixel((int)(j * rescaleFactor), (int)(i * rescaleFactor)));
            }
        }

        rtnTex.Apply();
        return rtnTex;
    }

    public Texture2D rotate90(Texture2D raw)
    {
        Texture2D rtnTexture = new Texture2D(raw.height, raw.width);
        for (int i = 0; i < raw.width - 1; i++)
        {
            for (int j = 0; j < raw.height - 1; j++)
            {
                Color color = raw.GetPixel(i, j);
                rtnTexture.SetPixel(j, raw.width - 1 - i, color);
            }
        }

        rtnTexture.Apply();
        return rtnTexture;
    }

    public string GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds).ToString();
    }

    private void OnDestroy()
    {
        WsClient.sharedWs.CloseConnect();
        frontCam.Stop();
    }

    public void refresh()
    {
        WsClient.sharedWs.sendRestart();
    }
}
