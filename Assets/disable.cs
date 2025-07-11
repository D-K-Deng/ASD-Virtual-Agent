using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disable : MonoBehaviour
{
    private CameraAccess cameraAccess;
    // Start is called before the first frame update
    void Start()
    {
        cameraAccess = this.gameObject.GetComponent<CameraAccess>();
        cameraAccess.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        cameraAccess.enabled = false;
    }
}
