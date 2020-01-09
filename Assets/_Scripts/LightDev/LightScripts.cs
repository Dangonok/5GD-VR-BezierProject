using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put it ont the object who activate the light!
/// </summary>
public class LightScripts : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Light")
        {
            if (other.GetComponent<LightDuration>().canBeTouch == true)
            {
                other.GetComponent<LightDuration>().canBeTouch = false;
                StartCoroutine(other.GetComponent<LightDuration>().ProgressifLight());
            }
        }
    }
}
