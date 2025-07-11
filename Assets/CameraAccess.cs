using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAccess : MonoBehaviour
{
    WebCamTexture webCamTexture;
    private Color[] GetPixels;
    //public string path;
    //public RawImage imDisplayForPhotoSnap;


    // Start is called before the first frame update
    void Start()
    {
        webCamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webCamTexture;
        webCamTexture.Play();
        //Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetPixels = webCamTexture.GetPixels();
        Debug.Log(GetPixels[3]);
    }

    
}
