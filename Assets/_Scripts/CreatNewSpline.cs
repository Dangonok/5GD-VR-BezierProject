using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class CreatNewSpline : MonoBehaviour
{
    [SerializeField] SplineComputer spline;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform handTransform;
    [SerializeField] SplineFollower splineFollower;

    private int nbrOfInput = 0;

    private void Awake()
    {
        //Create a new array of spline points
        SplinePoint[] points = new SplinePoint[20];
        //Set each point's properties
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = Vector3.forward * i* 20;
            points[i].normal = Vector3.up;
            points[i].size = 1f;
            points[i].color = Color.white;
        }
        //Write the points to the spline
        spline.SetPoints(points);
    }

    private void Start()
    {

    }

    private void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Mouse0))
        {
            nbrOfInput++;
            //new point
            double percentPrecedent = splineFollower.result.percent;
            double nbrPoints = (double)spline.pointCount;
            //Vector3 posPop = new Vector3(nbrOfInput, nbrOfInput, nbrOfInput);
            Vector3 posPop = GetInstanceDotPositionRay(handTransform, playerTransform);
            SplinePoint newPoint = new SplinePoint();
            newPoint.position = posPop;
            newPoint.normal = Vector3.up;
            newPoint.size = 1f;
            newPoint.color = Color.white;
            //CreateAPointClampSpline(newPoint);
            CreateAPoint(newPoint);
            spline.RebuildImmediate();
            splineFollower.result.percent = (double)(percentPrecedent / ((double)1 / (double)nbrPoints * ((double)nbrPoints + (double)1)));
        }
    }

    private void CreateAPointClampSpline(SplinePoint newPoint)
    {
        SplinePoint[] actualPoints = spline.GetPoints();
        SplinePoint[] newPoints = new SplinePoint[actualPoints.Length];
        //Set each point's properties
        // on doit parcourir tout le tableau précédent sauf le premier
        for (int i = 1; i < newPoints.Length; i++)
        {
            newPoints[i-1] = new SplinePoint();
            newPoints[i-1].position = actualPoints[i].position;
            newPoints[i-1].normal = actualPoints[i].normal;
            newPoints[i-1].size = actualPoints[i].size;
            newPoints[i-1].color = actualPoints[i].color;
        }
        //on add à la fin de la spline le nvx point
        newPoints[newPoints.Length-1] = newPoint;

        //Write the points to the spline
        spline.SetPoints(newPoints);
    }

    private void CreateAPoint(SplinePoint newPoint)
    {
        SplinePoint[] actualPoints = spline.GetPoints();
        SplinePoint[] newPoints = new SplinePoint[actualPoints.Length + 1];
        for (int i = 0; i < actualPoints.Length; i++)
        {
            newPoints[i] = actualPoints[i];
        }
        newPoints[newPoints.Length - 1] = newPoint;
        spline.SetPoints(newPoints);
    }

    private Vector3 GetInstanceDotPositionRay(Transform handTransform, Transform playerTransform)
    {
        Vector3 finalDotPosition = spline.GetPoint(spline.GetPoints().Length - 1).position;
        Vector3 focusRay = handTransform.forward * 30 + playerTransform.position;
        Vector3 pointToRayDirection = (focusRay - finalDotPosition).normalized;
        Vector3 newDotPosition = finalDotPosition + pointToRayDirection * GameManager.Instance.datas.distanceInit;
        return newDotPosition;
    }
}
