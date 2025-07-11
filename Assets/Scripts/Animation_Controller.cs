using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Random = UnityEngine.Random;
using UnityEngine.UI;


public class Animation_Controller : MonoBehaviour
{
 

    public const string audioName = "test3.wav";

    public GameObject teacher;
    private Animator animator;
    

    [Header("Audio Stuff")]
    public AudioSource audioSource;
    public AudioClip audioClip;
    public string soundPath;

    private bool play_audio = false;

    private float wait;

    string[] body_ges = { "Compare", "CasualTalk",
                        "Thinking", "Shrug","Emphasize" };

    string[] face_ges = {"Sympathy","Sad","Serious","Analysis",
                        "Normal", "Manner","SlightSmile","Smile","Joy"};

    private int rnd;

    private bool file_Found = false;

    private bool check_Folder = true;

    public static int counter;

    private string emo_input;

    private bool stop_thread = false;

    //the avatar's text shown on the screen
    public Text avatarText;

    //private HelloSender _helloSender;

    // Start is called before the first frame update
    void Start()
    {
        animator = teacher.GetComponent<Animator>();
        

        audioSource = gameObject.AddComponent<AudioSource>();
        soundPath = "file://" + Application.streamingAssetsPath + "/Recording/";

        //_helloSender = new HelloSender();

        //List<string> authorsRange = new List<string>(body_ges);


    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(LoadAudio());
        //    animator.SetBool("Smile", true);
        //    Debug.Log("Smile has been played");

        //    rnd = Random.Range(0, 5);
        //    animator.SetBool(body_ges[rnd], true);
        //    Debug.Log(body_ges[rnd] + "has been played");
        //}


        /*if (check_Folder)
        {
            //get the folder information of the avatar's sound
            string[] FilePaths = Directory.GetFiles(Application.streamingAssetsPath + "/Recording/", "*.wav", SearchOption.AllDirectories);


            if (FilePaths != null)
            {
                if (FilePaths.Length > 0)
                {
                    file_Found = true;
                    Debug.Log("File is found");
                    check_Folder = false;
                }
                else
                {
                    file_Found = false;

                }
            }
            else
            {
                Debug.Log("No array was returned!");
            }

            
        }

            
        if((file_Found) & (counter == 0))
        {
            //if the value of emotion is send back, play animation and audio
            //if(HelloSender.message!=null)
            if(NetMQClient.messageReceived != null)
            //if(_helloSender.ReceiveMessage()!=null)
            {
                Cope_Animation();
                counter = 1;
                StartCoroutine(AvatarBodyGestures());
                

                
                //Debug.Log("Emotion value is sent back!!!!!");
            }    
        }*/

        //if((NetMQClient.messageReceived!=null)&(counter == 0))
        //{
        //    Cope_Animation();
        //    counter = 1;
        //    StartCoroutine(AvatarBodyGestures());
        //}

        

        if (play_audio)
        {
            wait -= Time.deltaTime; //reverse count
        }

        if ((wait < 0f) & play_audio)
        { //here you can check if clip is not playing
            //Debug.Log("sound is end");

            //animator.SetBool("SmileHappy", false);
            //Debug.Log("Idle has stopped");

            //after the audio has stopped, the body gesture and facial expression turn back to idle
            animator.SetBool(face_ges[Map_face(emo_input)], false);
            //animator.SetBool(body_ges[rnd], false);
            animator.SetBool("Mouth Talk", false);

            play_audio = false;
            //Debug.Log("Audio has stopped");
            //stop_thread = true;
            //if(stop_thread)
            //{
            //    //Micro_Camera._helloRequester.Stop();
            //    stop_thread = false;
            //    Debug.Log("Thread has been stopped!");
            //}
        }

    }

    private IEnumerator AvatarBodyGestures()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            animator.SetInteger("BodyGesturesIndex", Random.Range(0, 5));
            animator.SetTrigger("BodyGestures");


            if (!play_audio)
            {
                //Debug.Log("The loop has ended");
                animator.SetTrigger("ExitSubState");
                animator.SetTrigger("BackToIdle");
                break;

            }

        }
    }


    private IEnumerator LoadAudio()
    {
        WWW request = GetAudioFromFile(soundPath, audioName);
        yield return request;

        audioClip = request.GetAudioClip();
        audioClip.name = audioName;

        PlayAudioFile();
        play_audio = true;

        //get the length of the audio clip
        

        
    }

    private void PlayAudioFile()
    {
        audioSource.clip = audioClip;
        audioSource.Play();

        //wait = audioClip.length;
        //audioSource.loop = true;
    }

    private WWW GetAudioFromFile(string path, string filename)
    {
        string audioToLoad = string.Format(path + "{0}", filename);
        WWW request = new WWW(audioToLoad);
        return request;
    }

    private void Cope_Animation()
    {

        StartCoroutine(LoadAudio());

        //randomly generate a body_gesture
        //rnd = Random.Range(0, 5);

        //mark that audio is been played
        play_audio = true;

        //animator.SetBool("SmileHappy", true);
        //Debug.Log("SmileHappy has been played");

        //split the sended back message into 3 elements, the first is length of the text, the second is the length of the audio, the third is the emotion value
        //string[] sended_back_message = HelloRequester.message.Split('\n');

        //string[] sended_back_message = NetMQClient.messageReceived.Split('\n');

        //string[] sended_back_message = _helloSender.ReceiveMessage().Split('\n');
        //emo_input = sended_back_message[2];
        Debug.Log("The emotional value is: " + emo_input);
        //get the length of the audio
        //wait = Single.Parse(sended_back_message[1]);
        Debug.Log("Wait for " + wait);
        //Debug.Log("Received Emotion Value " + sended_back_message[2]);

        animator.SetBool(face_ges[Map_face(emo_input)], true);

        //Debug.Log(face_ges[Map_face(emo_input)] + " has been played");

        //animator.SetBool(body_ges[rnd], true);
        //Debug.Log(body_ges[rnd] + " has been played");

        animator.SetBool("Mouth Talk", true);
        //Debug.Log("Mouth is talkiing!");

        //Debug.Log("Send back 4:" + sended_back_message[3][0]);

        //The avatar's text shown on the screen
        //avatarText.text = sended_back_message[4];



        //Micro_Camera._helloRequester.Stop();
    }

    //map the returned emotion value from the server to a face animation
    private int Map_face(string input_value)
    {
        float emo_val = Single.Parse(input_value);
        int face_ani = 4;
        if((-1<=emo_val)&(emo_val<-0.875))
        {
            face_ani = 0;          
        }
        
        else if ((-0.875 <= emo_val) & (emo_val < -0.625))
        {
            face_ani = 1;           
        }

        else if ((-0.625 <= emo_val) & (emo_val < -0.375))
        {
            face_ani = 2;
        }

        else if ((-0.375 <= emo_val) & (emo_val < -0.125))
        {
            face_ani = 3;
        }

        else if ((-0.125 <= emo_val) & (emo_val < 0.125))
        {
            face_ani = 4;
        }

        else if ((0.125 <= emo_val) & (emo_val < 0.375))
        {
            face_ani = 5;
        }

        else if ((0.375 <= emo_val) & (emo_val < 0.625))
        {
            face_ani = 6;
        }

        else if ((0.625 <= emo_val) & (emo_val < 0.875))
        {
            face_ani = 7;
        }

        else if ((0.875 <= emo_val) & (emo_val < 1))
        {
            face_ani = 7;
        }

        return face_ani;
    }
}
