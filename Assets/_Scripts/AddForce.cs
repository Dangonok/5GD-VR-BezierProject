using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
    [SerializeField] Vector3 force;
    [SerializeField] Vector3 angularforce;
    [SerializeField] Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb.AddForce(force);
        rb.AddTorque(angularforce);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
