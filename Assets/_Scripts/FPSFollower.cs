using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSFollower : MonoBehaviour
{
    [SerializeField] Transform follower;

    void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, follower.position, 0.33f);
        //transform.up = follower.up;
    }
}
