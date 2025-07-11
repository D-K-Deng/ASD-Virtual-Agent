using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Net;

public class MultiQues : MonoBehaviour
{
    private int pages;
    //Use a dictionary to store the sequence of the choices of a question
    //The keys are the page index and the values are tuples of the first 
    //element meaning the sequence (either 1 or 2) of the question page
    //and the bool means if the question has been answered correctly ot not
    private Dictionary<int, int> answeredSequences = new Dictionary<int, int>();

    private int currentRandomNumber;

    private GameObject lastButton;
    private GameObject nextButton;

    [Serializable]
    private class SpawnMultiQues
    {
        public GameObject[] questions = new GameObject[5];
    }

    [SerializeField] private SpawnMultiQues[] spawnMultiQuesPages;
    // Start is called before the first frame update
    void Start()
    {
        lastButton = GameObject.Find("Last");
        nextButton = GameObject.Find("Next");
        lastButton.SetActive(false);
        nextButton.SetActive(false);
        Debug.Log(pages);
        pages = 0;
        currentRandomNumber = Random.Range(1, 3);
        //Debug.Log(n);
        spawnMultiQuesPages[0].questions[0].SetActive(true);
        if (currentRandomNumber == 1)
        {
            spawnMultiQuesPages[0].questions[1].SetActive(true);
            spawnMultiQuesPages[0].questions[4].SetActive(true);
        }

        else
        {
            spawnMultiQuesPages[0].questions[2].SetActive(true);
            spawnMultiQuesPages[0].questions[3].SetActive(true);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (answeredSequences.ContainsKey(pages))
        {
            nextButton.SetActive(true);
        }

        else
        {
            nextButton.SetActive(false);
        }
    }

    public void CorretAns()
    {
        // Store the sequence in the dictionary
        if (!answeredSequences.ContainsKey(pages))
        {
            answeredSequences.Add(pages, currentRandomNumber);
        }

        if (!answeredSequences.ContainsKey(pages + 1))
        {
            RandomInstantiate(pages);
        }

        else
        {
            RestoreSequence(pages + 1, answeredSequences[pages + 1]);
        }
        /*
        for (int i = 0; i < spawnMultiQuesPages.Length - 1; i++)
        {
            if (spawnMultiQuesPages[i].questions[0].activeInHierarchy)
            {
                RandomInstantiate(i);
                break;
            }
        }
        */
        //Set the last button to visible in the scene
        if (!lastButton.activeInHierarchy)
        {
            lastButton.SetActive(true);
        }

        pages++;
        Debug.Log(pages);
    }



    private void RandomInstantiate(int i)
    {
        currentRandomNumber = Random.Range(1, 3);
        spawnMultiQuesPages[i].questions[0].SetActive(false);
        for (int j = 1; j < 5; j++)
        {
            if (spawnMultiQuesPages[i].questions[j].activeInHierarchy)
            {
                spawnMultiQuesPages[i].questions[j].SetActive(false);
            }
        }
        spawnMultiQuesPages[i + 1].questions[0].SetActive(true);
        if (currentRandomNumber == 1)
        {
            spawnMultiQuesPages[i + 1].questions[1].SetActive(true);
            spawnMultiQuesPages[i + 1].questions[4].SetActive(true);
        }

        else if (currentRandomNumber == 2)
        {
            spawnMultiQuesPages[i + 1].questions[2].SetActive(true);
            spawnMultiQuesPages[i + 1].questions[3].SetActive(true);
        }
    }

    //private void RandomInstantiateLastQues(int i)
    //{
    //    spawnMultiQuesPages[i].questions[0].SetActive(false);
    //    for (int j = 1; j < 5; j++)
    //    {
    //        if (spawnMultiQuesPages[i].questions[j].activeInHierarchy)
    //        {
    //            spawnMultiQuesPages[i].questions[j].SetActive(false);
    //        }
    //    }
    //    int n = Random.Range(1, 3);
    //    spawnMultiQuesPages[i - 1].questions[0].SetActive(true);
    //    if (n == 1)
    //    {
    //        spawnMultiQuesPages[i - 1].questions[1].SetActive(true);
    //        spawnMultiQuesPages[i - 1].questions[4].SetActive(true);
    //    }

    //    else if (n == 2)
    //    {
    //        spawnMultiQuesPages[i - 1].questions[2].SetActive(true);
    //        spawnMultiQuesPages[i - 1].questions[3].SetActive(true);
    //    }
    //}

    //This function is used to go back to the last question and restore the sequence of choices of that question
    public void PreviousQues()
    {
        if (pages > 0)
        {
            spawnMultiQuesPages[pages].questions[0].SetActive(false);
            for (int j = 1; j < 5; j++)
            {
                if (spawnMultiQuesPages[pages].questions[j].activeInHierarchy)
                {
                    spawnMultiQuesPages[pages].questions[j].SetActive(false);
                }
            }
            pages--;
            Debug.Log(pages);

            if (answeredSequences.ContainsKey(pages))
            {
                // Restore the sequence from the dictionary
                RestoreSequence(pages, answeredSequences[pages]);
                Debug.Log("The sequence is" + answeredSequences[pages]);
            }
            else
            {

                // Existing logic for going to the previous question
                // ...
            }

            if (pages == 0)
            {
                lastButton.SetActive(false);
            }
        }


        //if (pages > 0)
        //{
        //for (int i = 0; i < spawnMultiQuesPages.Length; i++)
        //{
        //    if (spawnMultiQuesPages[i].questions[0].activeInHierarchy)
        //    {
        //        RandomInstantiateLastQues(i);
        //        break;
        //    }
        //}
        //pages--;
        //Debug.Log(pages);
        //}
    }

    /*
    public void NextQues()
    {
        if(pages <= answeredSequences.Count )
        {
            spawnMultiQuesPages[pages].questions[0].SetActive(false);
            for (int j = 1; j < 5; j++)
            {
                if (spawnMultiQuesPages[pages].questions[j].activeInHierarchy)
                {
                    spawnMultiQuesPages[pages].questions[j].SetActive(false);
                }
            }
            pages++;
            Debug.Log(pages);

            if (answeredSequences.ContainsKey(pages))
            {
                // Restore the sequence from the dictionary
                RestoreSequence(pages, answeredSequences[pages]);
                Debug.Log("The sequence is" + answeredSequences[pages]);
            }
            else
            {

                // Existing logic for going to the previous question
                // ...
            }
        }
    }
    */



    private void RestoreSequence(int pageIndex, int sequence)
    {
        // Logic to restore the sequence of choices based on the sequence number
        if (sequence == 1)
        {
            spawnMultiQuesPages[pageIndex].questions[1].SetActive(true);
            spawnMultiQuesPages[pageIndex].questions[4].SetActive(true);
        }

        else if (sequence == 2)
        {
            spawnMultiQuesPages[pageIndex].questions[2].SetActive(true);
            spawnMultiQuesPages[pageIndex].questions[3].SetActive(true);
        }
    }
}