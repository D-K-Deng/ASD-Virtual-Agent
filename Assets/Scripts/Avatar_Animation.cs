using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
public class Avatar_Animation : MonoBehaviour
{
    public Scrollbar myScrollBar;

    public GameObject teacher;
    private Animator animator;
    private bool played = false;
    string[] face_ges = {"Sympathy","SadSerious","Sad","Serious",
                        "Normal", "Manner","Smile","SmileMiddle","SmileHappy"};
    /*
    string[] body_ges = { "Compare", "CasualTalk",
                        "Thinking", "Shrug","Emphasize" };
    */

    public Text avatarText;
    //this is the counter for making sure only calling AvatarBodyGest once a time
    private int counter = 0;
    private int finishe_talking_counter = 0;
    private float integrated_val;

    //private int num_of_clip = 0;

    //private bool start_body_ges = false;
    // Start is called before the first frame update
    void Start()
    {
        animator= teacher.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        // Set the rotation of the 'teacher' object to face the opposite direction.
        teacher.transform.rotation = Quaternion.Euler(teacher.transform.rotation.eulerAngles.x, 180f, teacher.transform.rotation.eulerAngles.z);


        //This part is for the facial expressions
        //&& (WsClient.face_emo != null)
        // && (WsClient.tempo_emo!=null)
        //This part is for the facial expressions


        if (Micro_Camera.start_asking_ques && WsClient.face_emo != null && WsClient.tempo_emo == null)
        {
            int faceIndex = Map_face(WsClient.face_emo, null);

            Debug.Log("Have facial expressions!!!");

            // Set the selected facial expression to true and all others to false
            for (int i = 0; i < face_ges.Length; i++)
            {
                animator.SetBool(face_ges[i], i == faceIndex);
            }
            //Debug.Log(face_ges[faceIndex] + " has been played");

            myScrollBar.value = 1f;
        }
        

        //avatarText.text = WsClient.printed_text;
        //Debug.Log("The list count is: " + PlaySound.instance.wav2Speak.Count);


        if ((counter == 0)  && WsClient.face_emo != null && (WsClient.tempo_emo != null))
        //if ((PlaySound.instance.audiosource.isPlaying)&&(counter==0)&&(num_of_clip < PlaySound.instance.wav2Speak.Count))
        {
            //&& (PlaySound.instance.audiosource.isPlaying)
            Debug.Log("这个可以跑！！！");

            avatarText.text = WsClient.printed_text;
            animator.SetBool("Mouth Talk", true);
            //Start the body gestures
            StartCoroutine(AvatarBodyGestures());
            //Make the avatar move mouse
            //set the counter to one so that this if statement would not be called on the next frame
            counter = 1;
            //wait = PlaySound.audio_length;
            //Debug.Log("The length for the playing sound is: " + wait + " It is the " + num_of_clip + "th clip in the list");

            //This is for the if statement in the AvatarBodyGesture() function to decide whether the body gesture go back to idle or not
            //start_body_ges=true;

            finishe_talking_counter= 0;
            
            int faceIndex = Map_face(WsClient.face_emo, WsClient.tempo_emo);

            // Set the selected facial expression to true and all others to false
            for (int i = 0; i < face_ges.Length; i++)
            {
                animator.SetBool(face_ges[i], i == faceIndex);
            }
            

        }
        /*
        if (wait>0)
        {
            wait -= Time.deltaTime; //reverse count
        }

        else
        {
            counter= 0;
            num_of_clip++;

        }
        */

        //close the avatar's mouth
        if((!PlaySound.instance.audiosource.isPlaying)&&(finishe_talking_counter==0))
        //if((!PlaySound.instance.audiosource.isPlaying)&&(PlaySound.finished_speaking))
        {
            //start_body_ges= false;
            //Set the mouse to false once all the audio clips hase been played
            animator.SetBool("Mouth Talk", false);

            //set the finishe_talking_counter to 1 to only excute once
            finishe_talking_counter= 1;

            //set the talking counter to 0
            counter = 0;
            Debug.Log("Mouth finished talking!!!!");

            int faceIndex = 4;

            // Set the selected facial expression to true and all others to false
            for (int i = 0; i < face_ges.Length; i++)
            {
                animator.SetBool(face_ges[i], i == faceIndex);
            }

            WsClient.face_emo = null;
            WsClient.tempo_emo = null;


        }
        

        /*
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
            // Set the selected facial expression to true and all others to false
            int faceIndex = 4;

            for (int i = 0; i < face_ges.Length; i++)
            {
                animator.SetBool(face_ges[i], i == faceIndex);
            }

            WsClient.face_emo = null;
            WsClient.tempo_emo = null;
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
        */
    }

    private IEnumerator AvatarBodyGestures()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            animator.SetInteger("BodyGesturesIndex", Random.Range(0, 5));
            animator.SetTrigger("BodyGestures");


            if (!PlaySound.instance.audiosource.isPlaying)
            {
                //Debug.Log("The loop has ended");
                animator.SetTrigger("ExitSubState");
                animator.SetTrigger("BackToIdle");
                break;

            }

        }
    }

    private int Map_face(string face_input_value, string tempo_input_value)
    {
        float emo_val = Single.Parse(face_input_value);

        //This is for the part where avatar is speaking
        if(tempo_input_value != null)
        {
            float tempo_val = Single.Parse(tempo_input_value);
            integrated_val = 0.5f * emo_val + 0.5f * tempo_val;
        }

        //When avatar is listening and giving facial expressions
        else
        {
            integrated_val = emo_val;
        }
        
        //Debug.Log("integrated vaue: " + integrated_val);
        int face_ani = 4;
        /*
        if(emo_val >= 0)
        {
            emo_val = emo_val * 2;
        }
        else
        {
            emo_val = emo_val * 3;
        }
        */
        if ((-1 <= integrated_val) & (integrated_val < -0.875))
        {
            face_ani = 0;
        }

        else if ((-0.875 <= integrated_val) & (integrated_val < -0.625))
        {
            face_ani = 1;
        }

        else if ((-0.625 <= integrated_val) & (integrated_val < -0.375))
        {
            face_ani = 2;
        }

        else if ((-0.375 <= integrated_val) & (integrated_val < -0.125))
        {
            face_ani = 3;
        }

        else if ((-0.125 <= integrated_val) & (integrated_val < 0.125))
        {
            face_ani = 4;
        }

        else if ((0.125 <= integrated_val) & (integrated_val < 0.375))
        {
            face_ani = 5;
        }

        else if ((0.375 <= integrated_val) & (integrated_val < 0.625))
        {
            face_ani = 6;
        }

        else if ((0.625 <= integrated_val) & (integrated_val < 0.875))
        {
            face_ani = 7;
        }

        else if ((0.875 <= integrated_val) & (integrated_val < 1))
        {
            face_ani = 8;
        }

        else if (integrated_val >= 1)
        {
            face_ani = 8;
        }

        else if(integrated_val <= -1)
        {
            face_ani = 0;
        }

        return face_ani;
    }
}

