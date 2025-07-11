using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlipPage : MonoBehaviour
{
    public AudioSource play_Audio;
    private bool break_Audio;
    private float inter_audio_len;
    private bool isPaused;

    [Serializable]
    private class SpawnGameObjects
    {
        public GameObject[] page = new GameObject[10];
        public AudioClip audio;
    }

    [SerializeField] private SpawnGameObjects[] spawnGameObjectsPages;

    private float audio_len;
    private bool play;
    private Animator animator;


    // Start is called before the first frame update
    void Awake()
    {
        animator = FindObjectOfType<Animator>();
        audio_len = spawnGameObjectsPages[0].audio.length;
        
    }

    private void Start()
    {
        play_Audio.Play();
        play = true;
        StartCoroutine(AvatarBodyGestures());
        animator.SetBool("Mouth Talk", true);
        isPaused = false;

    }

    //public void Press_to_play()
    //{
    //  play_Audio.Play();
    //play = true;
    //StartCoroutine(AvatarBodyGestures());
    //animator.SetBool("Mouth Talk", true);
    //}

    private IEnumerator AvatarBodyGestures()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            animator.SetInteger("BodyGesturesIndex", UnityEngine.Random.Range(0, 5));
            animator.SetTrigger("BodyGestures");



            if (!play)
            {
                Debug.Log("The loop has ended");
                animator.SetTrigger("ExitSubState");
                animator.SetTrigger("BackToIdle");
                break;

            }


        }
    }

    private void Update()
    {
        if (play)
        {
            audio_len -= Time.deltaTime;
        }

        if ((audio_len < 0f) & play)
        {
            play = false;
            animator.SetBool("Mouth Talk", false);
            audio_len = inter_audio_len;
        }
    }


    // This is the new TogglePause method
    public void TogglePause()
    {
        if (!isPaused)
        {
            play_Audio.Pause();
            animator.SetBool("Mouth Talk", false);
            animator.SetTrigger("BackToIdle");
            isPaused = true;
            play = false; // This will stop the body gestures as well
        }
        else
        {
            play_Audio.UnPause();
            animator.SetBool("Mouth Talk", true);
            isPaused = false;
            StartCoroutine(AvatarBodyGestures());
            play = true; // This will resume the body gestures as well
        }
    }


        public void NextPage()
    {

        for (int i = 0; i < spawnGameObjectsPages.Length - 1; i++)
        {
            if (spawnGameObjectsPages[i].page[0].activeInHierarchy)
            {
                for (int j = 0; j < spawnGameObjectsPages[i].page.Length; j++)
                {
                    spawnGameObjectsPages[i].page[j].SetActive(false);
                    spawnGameObjectsPages[i + 1].page[j].SetActive(true);
                    play_Audio.clip = spawnGameObjectsPages[i + 1].audio;
                    audio_len = spawnGameObjectsPages[i + 1].audio.length;
                    inter_audio_len = audio_len;
                }

                //animator.SetTrigger("ExitSubState");
                //animator.SetTrigger("BackToIdle");
                break;
            }
        }
    }

    public void PreviousPage()
    {
        break_Audio = true;
        for (int i = spawnGameObjectsPages.Length - 1; i > 0; i--)
        {
            if (spawnGameObjectsPages[i].page[0].activeInHierarchy)
            {
                for (int j = 0; j < spawnGameObjectsPages[i].page.Length; j++)
                {
                    spawnGameObjectsPages[i].page[j].SetActive(false);
                    spawnGameObjectsPages[i - 1].page[j].SetActive(true);
                    play_Audio.clip = spawnGameObjectsPages[i - 1].audio;
                    audio_len = spawnGameObjectsPages[i - 1].audio.length;
                }
                break;
            }
        }
    }
}