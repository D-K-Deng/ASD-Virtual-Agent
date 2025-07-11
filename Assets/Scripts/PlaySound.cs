using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlaySound : MonoBehaviour
{
    public AudioSource audiosource;
    AudioClip _IntroduceClip;
    public static PlaySound instance;
    public static float audio_length = 0;
    public static bool finished_speaking = false;
    // public bool soundReady = false;

    void Awake()
    {
        audiosource = GameObject.Find("TeacherSound").GetComponent<AudioSource>();
        // AudioConfiguration config = AudioSettings.GetConfiguration();
        // Debug.Log("Current audio output mode: " + config.speakerMode);
        // soundReady = true;
        // audiosource.playOnAwake = false; // If false, must call Play() manually
        instance = this; // Use PlaySound.instance to access
    }

    // Play online audio using WWW (legacy)
    public void LoadMusic(string filepath)
    {
        WWW www = new WWW(filepath);
        if (www.error != null)
        {
            Debug.Log(www.error);
        }
        AudioClip ac = www.GetAudioClip(false, false, AudioType.MPEG);
        audiosource.clip = ac;
        audiosource.Play();
    }

    // Load and play MP3 via UnityWebRequest
    public void LoadMp3(string filepath)
    {
        StartCoroutine(GetAudioClip(filepath));
    }

    // Stop current playback
    public void StopMusic()
    {
        if (audiosource.isPlaying)
        {
            audiosource.Stop();
        }
    }

    // Coroutine: fetch and play an audio clip from a URL
    IEnumerator GetAudioClip(string url)
    {
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(uwr.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
            audiosource.clip = clip;
            audiosource.Play();
        }
    }
}
