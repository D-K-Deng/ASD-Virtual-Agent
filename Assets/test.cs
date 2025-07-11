using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public bool start = false;
    public bool tony = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(start)
        {
            if (tony)
            {
                Debug.Log("Tony is true");
            }

            else
            {
                Debug.Log("Tony is false");
            }
        }
        
    }
}
