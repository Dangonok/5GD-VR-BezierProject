using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;
public class FPSSpline : MonoBehaviour
{
    [SerializeField] SplineComputer spline;
    [SerializeField] SplineFollower splineFollower;
    [SerializeField] Transform guizmoRotation;
    private float rotationZ;
    [SerializeField] Transform bufferRotation;
    [SerializeField] Transform curveInstantiator;
    [SerializeField] SplineComputer previsualisationSpline;

    void PreInstantiate()
    {
        SplinePoint[] points = new SplinePoint[GameManager.Instance.datas.howManySegmentAtTheBeginning];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = Vector3.forward * i * GameManager.Instance.datas.distanceBetweenAnchor;
            points[i].normal = Vector3.up;
            points[i].size = 1f;
            points[i].color = Color.white;
            if (i == 0)
            {
                points[i].tangent = -Vector3.forward * GameManager.Instance.datas.distanceBetweenAnchor / 2f + points[i].position;
                points[i].tangent2 = Vector3.forward * GameManager.Instance.datas.distanceBetweenAnchor / 2f + points[i].position;
            }
            else
            {
                Vector3 tangentDistance = points[i - 1].tangent2 - points[i].position;
                points[i].tangent = tangentDistance + points[i].position;
                points[i].tangent2 = -tangentDistance + points[i].position;
            }
        }
        spline.SetPoints(points);
    }

    void InstantiateANewAnchor(bool previsualisation)
    {
        if (previsualisation == false)
        {
            double percentPrecedent = splineFollower.result.percent;
            double nbrPoints = (double)spline.pointCount;
            SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);
            Vector3 posPop = GetInstanceDotPositionRay();
            SplinePoint newPoint = new SplinePoint();
            newPoint.position = posPop;
            newPoint.normal = curveInstantiator.up;
            newPoint.size = 1f;
            newPoint.color = Color.white;
            Vector3 distance = lastPoint.tangent2 - newPoint.position;
            newPoint.tangent = distance / 2f + newPoint.position;
            newPoint.tangent2 = -distance / 2f + newPoint.position;
            CreateAPoint(newPoint);
            spline.RebuildImmediate();
            splineFollower.result.percent = (double)(percentPrecedent / ((double)1 / (double)nbrPoints * ((double)nbrPoints + (double)1)));
        }
        else
        {
            SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);
            SplinePoint[] points = new SplinePoint[2];
            points[0] = lastPoint;
            Vector3 newPos = GetInstanceDotPositionRay();
            points[1].position = newPos;
            points[1].normal = curveInstantiator.transform.up;
            points[1].size = 1f;
            points[1].color = Color.red;
            Vector3 distance = points[0].tangent2 - points[1].position;
            points[1].tangent = distance / 1.5f + points[1].position;
            points[1].tangent2 = -distance / 1.5f + points[1].position;
            previsualisationSpline.SetPoints(points);
        }
    }

    private Vector3 GetInstanceDotPositionRay()
    {
        SetBuffer(guizmoRotation);
        Vector3 finalDotPosition = spline.GetPoint(spline.pointCount -1).position;
        Vector3 focusRay = transform.forward * 200 + transform.position;
        Vector3 pointToRayDirection = (focusRay - finalDotPosition).normalized;
        Vector3 newDotPosition = finalDotPosition + pointToRayDirection * GameManager.Instance.datas.distanceInit;
        return newDotPosition;
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
    
    private void GuizmoRotation(float rotation)
    {
        rotationZ += Time.deltaTime * -rotation * 30000f;
        rotationZ = Mathf.Clamp(rotationZ, -80f, 80f);
        guizmoRotation.localRotation = Quaternion.Lerp(guizmoRotation.localRotation, Quaternion.Euler(new Vector3(0, 0, rotationZ)), 0.33f);
    }

    private void SetBuffer(Transform handTransform)
    {
        SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);
        bufferRotation.transform.position = lastPoint.position;
        bufferRotation.transform.rotation = Quaternion.LookRotation(lastPoint.tangent2 - lastPoint.position, lastPoint.normal);
        curveInstantiator.transform.localRotation = handTransform.localRotation;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            InstantiateANewAnchor(false);
        }

        if (GameManager.Instance.datas.previsualisation)
            InstantiateANewAnchor(true);

        GuizmoRotation(Input.GetAxis("Mouse ScrollWheel"));
    }

    void Start()
    {

    }
}
