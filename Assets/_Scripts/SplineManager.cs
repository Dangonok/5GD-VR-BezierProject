using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Valve.VR;


public class SplineManager : MonoBehaviour
{
    [SerializeField] SplineComputer spline;
    [SerializeField] SplineComputer previsualisationSpline;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform handTransform;
    [SerializeField] SplineFollower splineFollower;


    [SerializeField] GameObject curveInstantiator;
    [SerializeField] GameObject BufferRotation;

    public SteamVR_Action_Boolean triggerAction;
    public SteamVR_Action_Boolean deleteTrigger;

    private float lastTimeInstant = 0;


    [SerializeField] Material baseMaterialPrevisualisation;
    [SerializeField] Material BlockedMaterialPrevisualisation;

    bool waitForEndOfFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.datas.preInstantiate == true)
        {
            PreInstantiate();
            print("Preinstantiate");
        }
        spline.SetPointSize(0, 0.1f);
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
                points[i].tangent = -Vector3.forward * GameManager.Instance.datas.distanceBetweenAnchor/1.5f + points[i].position;
                points[i].tangent2 = Vector3.forward * GameManager.Instance.datas.distanceBetweenAnchor/1.5f + points[i].position;
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
        //CheckForEndOfPath();
        if (spline.pointCount > Mathf.Infinity)
        {
            DestroyTheFirstX(1);
        }
        
        if (triggerAction.GetStateDown(SteamVR_Input_Sources.Any) /*&& lastTimeInstant + 1f < Time.time*/)
        {
            if (GameManager.Instance.datas.infinityPath == true)
            {
                InstantiateANewAnchor(false, null);
            }
            else
            {
                if (1f - 1f/spline.pointCount < splineFollower.result.percent)
                {
                    InstantiateANewAnchor(false, null);
                }
            }
            //lastTimeInstant = Time.time;
        }

        if (deleteTrigger.GetStateDown(SteamVR_Input_Sources.Any) && GameManager.Instance.datas.infinityPath == true )
        {
            DestroyTheLastX(1);
        }


        if (GameManager.Instance.datas.previsualisation /*&& lastTimeInstant + 1f < Time.time*/)
        {
            if (1f - 1f / spline.pointCount < splineFollower.result.percent)
            {
                MakeThePevisualisation(baseMaterialPrevisualisation);
            }
            else
            {
                MakeThePevisualisation(BlockedMaterialPrevisualisation);
            }
            //previsualisationSpline.gameObject.SetActive(true);
        }
        else
        {
            //previsualisationSpline.gameObject.SetActive(false);
        }
        Vector3 test = GetNewPosition(handTransform);
    }


    void MakeThePathSmaller()
    {
        for (float i = 0; i < spline.pointCount - 5 ; i++)
        {
            print("done it");
            float value = 1 - ((spline.pointCount - 5 - i) / 40);
            if (value < 0.01f)
            {
                value = 0.01f;
            }
            print(value + "    " + i);
            spline.SetPointSize((int)i, value);
        }
    }

    

    private void LateUpdate()
    {
        if (waitForEndOfFrame == true)
        {
            StartCoroutine(Rebuild());
            waitForEndOfFrame = false;
        }
    }

    IEnumerator Rebuild()
    {
        yield return 0;
        spline.RebuildImmediate();
    }


    void CheckForEndOfPath()
    {
        double numberOfPoint = spline.pointCount;
        double pourcentageOfOnePoint = (double) 100f / numberOfPoint;
        double endOfTheLastPoint = (double)pourcentageOfOnePoint -  (double)pourcentageOfOnePoint * (GameManager.Instance.datas.PourcentageOfLastSegment / (double)100f);
        double valueToDrawAnotherPoint = ((double)100f - endOfTheLastPoint)/(double)100;

        if (splineFollower.result.percent > valueToDrawAnotherPoint)
        {

            InstantiateForEndOfRoad();
        }
    }

    double CheckThePercentPointRelatif()
    {
        double numberOfPoint = spline.pointCount;
        double pourcentageOfOnePoint = (double)100f / numberOfPoint;
        double endOfTheLastPoint = (double)pourcentageOfOnePoint - (double)pourcentageOfOnePoint * (GameManager.Instance.datas.PourcentageOfLastSegment / (double)100f);
        double valueToDrawAnotherPoint = ((double)100f - endOfTheLastPoint) / (double)100;
        return valueToDrawAnotherPoint;
    }



    void MakeThePevisualisation(Material material)
    {
        InstantiateANewAnchor(true, material);
    }

    public void DestroyTheLastX(int numberOfSegment)
    {
        SplinePoint[] actualPoints = spline.GetPoints();
        SplinePoint[] newPoints = new SplinePoint[actualPoints.Length - numberOfSegment];
        for (int i = 0; i < newPoints.Length; i++)
        {
            newPoints[i] = actualPoints[i];
        }
        spline.SetPoints(newPoints);
        waitForEndOfFrame = true;
        splineFollower.result.percent = (double)(splineFollower.result.percent * ((double)1 + (double)1 / (double)spline.pointCount));
    }

    public void DestroyTheFirstX(int numberOfSegment)
    {
        SplinePoint[] actualPoints = spline.GetPoints();
        SplinePoint[] newPoints = new SplinePoint[actualPoints.Length - numberOfSegment];
        for (int i = 0; i < newPoints.Length; i++)
        {
            newPoints[i] = actualPoints[i+1];
        }
        spline.SetPoints(newPoints);
        waitForEndOfFrame = true;
        print(splineFollower.result.percent + "   " + spline.pointCount);
        print ((double)spline.pointCount / (double)spline.pointCount + (double)1);
        splineFollower.result.percent = (double)(splineFollower.result.percent * ((double)spline.pointCount / ((double)spline.pointCount +(double)1)));
        print(splineFollower.result.percent);
    }

    void InstantiateForEndOfRoad()
    {
        //new point
        SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);

        Vector3 newPos = GetNewPosition(spline.GetPoint(spline.pointCount -1).tangent2 - spline.GetPoint(spline.pointCount -1).position);
        SplinePoint newPoint = new SplinePoint();
        newPoint.position = newPos;
        newPoint.normal = curveInstantiator.transform.up;
        newPoint.size = 1f;
        newPoint.color = Color.white;
        Vector3 distance = lastPoint.tangent2 - newPoint.position;
        newPoint.tangent = distance / 1.5f + newPoint.position;
        newPoint.tangent2 = -distance / 1.5f + newPoint.position;

        //CreateAPointClampSpline(newPoint);
        CreateAPoint(newPoint);
        spline.RebuildImmediate();
        splineFollower.result.percent = (double)(splineFollower.result.percent / ((double)1 / (double)(double)spline.pointCount
                                        * ((double)(double)spline.pointCount + (double)1)));
        MakeThePathSmaller();
    }

    void InstantiateANewAnchor(SplinePoint buffer)
    {
        SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);

        Vector3 newPos = GetNewPosition(handTransform);
        SplinePoint newPoint = buffer;
        
        Vector3 distance = lastPoint.tangent2 - newPoint.position;
        newPoint.tangent = distance / 1.5f + newPoint.position;
        newPoint.tangent2 = -distance / 1.5f + newPoint.position;

        //CreateAPointClampSpline(newPoint);
        CreateAPoint(newPoint);
        spline.RebuildImmediate();
        splineFollower.result.percent = (double)(splineFollower.result.percent / ((double)1 / (double)(double)spline.pointCount
                                        * ((double)(double)spline.pointCount + (double)1)));
        MakeThePathSmaller();

    }

    void InstantiateANewAnchor(bool previsualisation, Material material)
    {
        if (previsualisation == false)
        {
            //new point
            SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);

            Vector3 newPos = GetNewPosition(handTransform);
            SplinePoint newPoint = new SplinePoint();
            newPoint.position = newPos;
            newPoint.normal = curveInstantiator.transform.up;
            newPoint.size = 0.5f;
            newPoint.color = Color.white;
            Vector3 distance = lastPoint.tangent2 - newPoint.position;
            newPoint.tangent = distance / 1.5f + newPoint.position;
            newPoint.tangent2 = -distance / 1.5f + newPoint.position;

            //CreateAPointClampSpline(newPoint);
            CreateAPoint(newPoint);
            spline.RebuildImmediate();
            splineFollower.result.percent = (double)(splineFollower.result.percent / ((double)1 / (double)(double)spline.pointCount
                                            * ((double)(double)spline.pointCount + (double)1)));
            MakeThePathSmaller();
        }
        else
        {
            SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);
            SplinePoint[] points = new SplinePoint[2];

            points[0] = lastPoint;

            Vector3 newPos = GetNewPosition(handTransform);
            points[1].position = newPos;
            points[1].normal = curveInstantiator.transform.up;
            points[1].size = 1f;
            points[1].color = Color.red;
            Vector3 distance = points[0].tangent2 - points[1].position;
            points[1].tangent = distance / 1.5f + points[1].position;
            points[1].tangent2 = -distance / 1.5f + points[1].position;
            previsualisationSpline.gameObject.GetComponent<Renderer>().material = material;
            previsualisationSpline.SetPoints(points);
        }
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

    Vector3 GetNewPosition (Vector3 rotationOfPoint)
    {
        SplinePoint lastPoint = spline.GetPoint(spline.pointCount - 1);
        BufferRotation.transform.position = lastPoint.position;
        BufferRotation.transform.rotation = Quaternion.LookRotation(lastPoint.tangent2 - lastPoint.position, lastPoint.normal);
        curveInstantiator.transform.localRotation = new Quaternion(0, 0, 0, 0) ;
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
