using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
public class CustomFollower : MonoBehaviour
{
    [SerializeField] SplineComputer spline;
    public double percent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = spline.EvaluatePosition(percent);
    }
}
