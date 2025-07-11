using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToNextText : MonoBehaviour
{
    public GameObject text0;
    public GameObject text1;
    public GameObject text2;
    public GameObject text3;

    public void NextText()
    {
        if(text0.activeInHierarchy)
        {
            text0.SetActive(false);
            text1.SetActive(true);
        }

        else if(text1.activeInHierarchy)
        {
            text1.SetActive(false);
            text2.SetActive(true);
        }

        else if (text2.activeInHierarchy)
        {
            text2.SetActive(false);
            text3.SetActive(true);
        }

    }

    public void PreviousText()
    {
        if (text1.activeInHierarchy)
        {
            text1.SetActive(false);
            text0.SetActive(true);
        }

        else if (text2.activeInHierarchy)
        {
            text2.SetActive(false);
            text1.SetActive(true);
        }

        else if (text3.activeInHierarchy)
        {
            text3.SetActive(false);
            text2.SetActive(true);
        }

    }
}
