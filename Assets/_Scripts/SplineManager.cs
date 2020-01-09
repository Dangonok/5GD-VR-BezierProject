using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Valve.VR;


public class SplineManager : MonoBehaviour
{
    [SerializeField] SplineComputer spline;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform handTransform;
    [SerializeField] SplineFollower splineFollower;


    [SerializeField] GameObject curveInstantiator;
    [SerializeField] GameObject BufferRotation;

    public SteamVR_Action_Boolean triggerAction;


    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.datas.preInstantiate == true)
        {
            PreInstantiate();
            print("Preinstantiate");
        }
    }

    void PreInstantiate()
    {
        //Create a new array of spline points
        SplinePoint[] points = new SplinePoint[GameManager.Instance.datas.howManySegmentAtTheBeginning];
        //Set each point's properties
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = Vector3.forward * i * GameManager.Instance.datas.distanceBetweenAnchor;
            points[i].normal = Vector3.up;
            points[i].size = 1f;
            points[i].color = Color.white;
            if (i == 0)
            {
                points[i].tangent = -Vector3.forward * GameManager.Instance.datas.distanceBetweenAnchor/2f + points[i].position;
                points[i].tangent2 = Vector3.forward * GameManager.Instance.datas.distanceBetweenAnchor/2f + points[i].position;
            }
            else
            {
                Vector3 tangentDistance = points[i - 1].tangent2 - points[i].position;
                points[i].tangent = tangentDistance + points[i].position;
                points[i].tangent2 = -tangentDistance + points[i].position;
            }
        }

        //Write the points to the spline
        spline.SetPoints(points);
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerAction.GetStateDown(SteamVR_Input_Sources.Any))
        {
            InstantiateANewAnchor();
        }

        if (GameManager.Instance.datas.previsualisation)
            MakeThePevisualisation();
        Vector3 test = GetNewPosition(handTransform);
    }

    void MakeThePevisualisation()
    {
        
    }


    void InstantiateANewAnchor()
    {
        //new point
        double percentPrecedent = splineFollower.result.percent;
        double nbrPoints = (double)spline.pointCount;
        SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);

        Vector3 posPop = GetNewPosition(handTransform);
        SplinePoint newPoint = new SplinePoint();
        newPoint.position = posPop;
        newPoint.normal = curveInstantiator.transform.up;
        newPoint.size = 1f;
        newPoint.color = Color.white;
        Vector3 distance = lastPoint.tangent2 - newPoint.position;
        newPoint.tangent = distance/2f + newPoint.position;
        newPoint.tangent2 = -distance/2f + newPoint.position;

        //CreateAPointClampSpline(newPoint);
        CreateAPoint(newPoint);
        spline.RebuildImmediate();
        splineFollower.result.percent = (double)(percentPrecedent / ((double)1 / (double)nbrPoints * ((double)nbrPoints + (double)1)));
    }

    Vector3 GetNewPosition(Transform handTransform)
    {
        SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);
        BufferRotation.transform.position = lastPoint.position;
        BufferRotation.transform.rotation = Quaternion.LookRotation(lastPoint.tangent2 - lastPoint.position, lastPoint.normal);
        curveInstantiator.transform.localRotation = handTransform.localRotation;
      // curveInstantiator.transform.localRotation = Quaternion.Lerp(Quaternion.identity, handTransform.localRotation , 0.5f);
        Vector3 newPos = curveInstantiator.transform.forward * GameManager.Instance.datas.distanceBetweenAnchor + BufferRotation.transform.position;
        return newPos;
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
}
