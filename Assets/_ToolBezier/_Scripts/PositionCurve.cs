using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCurve : MonoBehaviour
{

    [Header("Preinstantiate The Path")]
    [Tooltip("True if you want a predraw (Predraw destroy the path if there is one)")]
    public bool preInstantiate;
    [Tooltip("How many segment you want to draw at the beginning of the game")]
    public int howManySegment;

    [Header("How The Path Is Draw")]
    [Tooltip("How Many point are produce between two anchor")]
    public float smoothness;
    [Tooltip("Is The Path Draw Progressivily")]
    public bool progressifInstantiateRoad;
    [Tooltip("If progressifInstantiateRoad is true, how fast the road is draw")]
    public float speedOfDraw;
    [Tooltip("Position Between Each Point")]
    public int distanceBetweenAnchor;
    [Tooltip("If you want to see the next segment draw")]
    public bool previsualisation = false;


    [Header("RotationTest")]
    public float rotationValue;
    public float rotationOfTheLastRoadPart;


    [Header("Not Tooltiped yet or other Stuff")]

    [SerializeField] Transform handTransform;
    public GameObject curveHolder;
    public GameObject anchor;
    public GameObject physicalRoad;
    public GameObject roadHolder;
    public GameObject Instanciator;

    GameObject bufferRoad;

    public Vector3 posOfTheNewPoints;

    bool waitForTheEndOfPreinitialising = false;

    public List<Vector3> positions = new List<Vector3>();




    [SerializeField]
    public List<StructSegment> curveTab;

    // Start is called before the first frame update
    void Start()
    {
        if (preInstantiate == true)
            InitialisationOfThePath();

        /* if (previsualisation == true)
         {
             InstantiateNewPoints();
         }*/
    }

    // Update is called once per frame
    void Update()
    {
        /*if (previsualisation == true)
        {
            PathPrevisualisation();
        }*/
    }

    public void PathPrevisualisation()
    {
        DestroyTheEndOfThePathFrom(1);
        DestroyTheEndOfTheRoadFrom(1);
        DestroyTheEndOfThePositionFrom(1);
        InstantiateNewPoints();
    }

    public void PathPrevisualisation(Vector3 newPos)
    {
        DestroyTheEndOfThePathFrom(1);
        DestroyTheEndOfTheRoadFrom(1);
        DestroyTheEndOfThePositionFrom(1);
        InstantiateNewPoints(newPos, true);
    }



    public void DestroyTheEndOfThePositionFrom(int HowManySegment)
    {
        for (int i = 0; i < HowManySegment; i++)
        {
            for (int j = 0; j < smoothness; j++)
            {
                positions.Remove(positions[positions.Count - 1]);
            }
        }
    }

    public void DestroyTheEndOfTheRoadFrom(int HowManySegment)
    {
        for (int i = 0; i < HowManySegment; i++)
        {
            Destroy(roadHolder.transform.GetChild(roadHolder.transform.childCount - 1).gameObject);
        }
    }

    public void DestroyTheEndOfThePathFrom(int howManySegment)
    {
        for (int i = 0; i < howManySegment; i++)
        {
            Destroy(curveTab[curveTab.Count - 1].pointsAnchors.gameObject);
            Destroy(curveTab[curveTab.Count - 1].pointsAfterAnchors.gameObject);
            curveTab.Remove(curveTab[curveTab.Count - 1]);
        }
    }



    public void InitialisationOfThePath()
    {
        waitForTheEndOfPreinitialising = true;
        int curveChild = curveHolder.transform.childCount;
        for (int i = 0; i < curveChild; i++)
        {
            Destroy(curveHolder.transform.GetChild(0));
        }

        DestroyTheRoad();
        EmptyTheStructCurve();

        for (int i = 0; i < howManySegment; i++)
        {
            InstantiateNewPoints(new Vector3(0 , 0, 0 + distanceBetweenAnchor * i), false);
        }
        waitForTheEndOfPreinitialising = false;
    }

    public void EmptyTheStructCurve()
    {
        curveTab.Clear();
    }

    public void DestroyTheRoad()
    {
        int roadChild = roadHolder.transform.childCount;
        for (int i = 0; i < roadChild; i++)
        {
            Destroy(roadHolder.transform.GetChild(0).gameObject);
        }
    }

    //Permet de poser un nouveau point dans la scène
    public void InstantiateNewPoints()
    {
        print("Instanciate new point");
        GameObject newPoint = Instantiate(anchor, Instanciator.transform.position, Quaternion.identity);
        newPoint.name = "Point" + curveTab.Count.ToString();
        newPoint.transform.parent = curveHolder.transform;
        Vector3 posOfNextPoint;
        if (curveTab.Count == 0)
        {
            posOfNextPoint = new Vector3(0, 0, distanceBetweenAnchor * 0.5f);
        }
        else
        {
            posOfNextPoint = Vector3.LerpUnclamped(curveTab[curveTab.Count - 1].pointsAfterAnchors.transform.position, newPoint.transform.position, 2f);
        }

        GameObject pointAfterAnchor = Instantiate(anchor, posOfNextPoint, Quaternion.identity);
        pointAfterAnchor.transform.parent = curveHolder.transform;
        pointAfterAnchor.name = "Point" + curveTab.Count.ToString() + "AfterAnchor";

        AddToTheCurve(newPoint, pointAfterAnchor, previsualisation);
    }

    public void InstantiateNewPoints(Vector3 positionOfTheAnchor, bool previsualisation)
    {
        GameObject newPoint = Instantiate(anchor, positionOfTheAnchor, Quaternion.identity);
        newPoint.name = "Point" + curveTab.Count.ToString();
        newPoint.transform.parent = curveHolder.transform;

        Vector3 posOfNextPoint;
        if (curveTab.Count == 0)
        {
            posOfNextPoint = new Vector3(0, 0, distanceBetweenAnchor * 0.5f);
        }
        else
        {
            posOfNextPoint = Vector3.LerpUnclamped(curveTab[curveTab.Count - 1].pointsAfterAnchors.transform.position, newPoint.transform.position, 2f);
        }

        GameObject pointAfterAnchor = Instantiate(anchor, posOfNextPoint, Quaternion.identity);
        pointAfterAnchor.transform.parent = curveHolder.transform;
        pointAfterAnchor.name = "Point" + curveTab.Count.ToString() + "AfterAnchor";

        AddToTheCurve(newPoint, pointAfterAnchor, previsualisation);
    }

    //Permet de mettre un nouveau point dans le tableau de structure!
    void AddToTheCurve(GameObject anchor, GameObject afterAnchor, bool previsualisation)
    {
        if (curveTab.Count > 0)
        {
            curveTab.Add(new StructSegment
            {
                pointsAnchors = anchor.transform,
                pointsBeforeAnchors = curveTab[curveTab.Count - 1].pointsAfterAnchors,
                pointsAfterAnchors = afterAnchor.transform
            });
        }
        else
        {
            curveTab.Add(new StructSegment
            {
                pointsAnchors = anchor.transform,
                pointsAfterAnchors = afterAnchor.transform
            });
        }

        if (progressifInstantiateRoad == true && waitForTheEndOfPreinitialising == false)
            StartCoroutine(DrawNewSegment(speedOfDraw));
        else
        {
            DrawNewSegment(previsualisation);
        }
    }


    //Permet de faire la curve proprement
    Vector3 GetPoint(StructSegment CurveToDraw, StructSegment CurveBefore, float time)
    {
        Vector3 pos = Vector3.Lerp(Vector3.Lerp(CurveBefore.pointsAnchors.transform.position, CurveToDraw.pointsBeforeAnchors.transform.position, time),
                            Vector3.Lerp(CurveToDraw.pointsBeforeAnchors.transform.position, CurveToDraw.pointsAnchors.transform.position, time), time);
        return pos;
    }

    //Permet d'afficher la courbe
    void DrawNewSegment(bool previsualisation)
    {
        GameObject holderSegmentRoad = Instantiate(anchor, Vector3.zero, new Quaternion(0, 0, 0, 0), roadHolder.transform);
        holderSegmentRoad.name = "HolderRoadSegment(" + roadHolder.transform.childCount + ")";
        //Only Working for last segment
        Transform lastValidTransform = null;
        if (roadHolder.transform.childCount > 1)
        {
            Transform lastRoad = roadHolder.transform.GetChild(roadHolder.transform.childCount - 2);
            int lastChild = (int)smoothness - 1;
            if (lastRoad != null && lastRoad.childCount > lastChild)
            {
                lastValidTransform = lastRoad.GetChild(lastChild);
            }
        }
        
        for (float i = 0; i < smoothness; i++)
        {
            if (curveTab.Count != 1)
            {
                GameObject roadJustCreated = Instantiate(physicalRoad, GetPoint(curveTab[curveTab.Count - 1], curveTab[curveTab.Count - 2], (i / (float)smoothness)), new Quaternion (0,0,0,0), holderSegmentRoad.transform);
                roadJustCreated.name = "PartOfRoad: " + i;
                if (bufferRoad == null) 
                {
                    bufferRoad = roadJustCreated;
                }
                else
                {

                    Vector3 direction = (roadJustCreated.transform.position - bufferRoad.transform.position).normalized;
                     Quaternion quatCible = Quaternion.FromToRotation(bufferRoad.transform.forward, direction) * bufferRoad.transform.rotation;
                     bufferRoad.transform.rotation = quatCible;

                     
                    //Vector3 up = Vector3.Slerp(roadHolder.transform.GetChild(roadHolder.transform.childCount - 1).GetChild(0).transform.up, Instanciator.transform.up, (i / (float)smoothness));
                    //Vector3 up = Vector3.up;
                    //Vector3 fwd = roadJustCreated.transform.position - bufferRoad.transform.position;
                    
                    //bufferRoad.transform.localRotation = Quaternion.LookRotation(fwd, up);
                    //Debug.LogFormat("road {0} fwd fed:{1}  fwd tr:{2} up:{3}", i, fwd, bufferRoad.transform.forward, bufferRoad.transform.up);
                    /*bufferRoad.transform.LookAt(roadJustCreated.transform);
                    if(lastValidTransform != null)
                    {
                        Debug.LogFormat("testing alignment of {0} and {1}", bufferRoad.transform.parent.name + ">" + bufferRoad.name, lastValidTransform.parent.name + ">" + lastValidTransform.name);
                        float alignment = Vector3.Dot(bufferRoad.transform.up, lastValidTransform.up);
                        if(alignment<0)
                        {
                            Debug.LogFormat("{0} and {1} had opposite up vectors", bufferRoad.transform.parent.name + ">" + bufferRoad.name, lastValidTransform.parent.name + ">" + lastValidTransform.name);
                            bufferRoad.transform.Rotate(0, 0, 180);
                        }
                    }*/
                    //lastValidTransform = bufferRoad.transform;

                    //bufferRoad.transform.localRotation = Quaternion.Euler(bufferRoad.transform.rotation.eulerAngles.x, bufferRoad.transform.rotation.eulerAngles.y, (i / smoothness) * rotationValue + rotationOfTheLastRoadPart);
                    if (i == smoothness - 1)
                    {
                        /*roadJustCreated.transform.localRotation = Quaternion.Euler(bufferRoad.transform.rotation.eulerAngles.x, bufferRoad.transform.rotation.eulerAngles.y,
                                                                  bufferRoad.transform.rotation.eulerAngles.z + (1/smoothness) * rotationOfTheLastRoadPart);*/
                        roadJustCreated.transform.localRotation = bufferRoad.transform.localRotation;
                    }
                    bufferRoad = roadJustCreated;
                }
                positions.Add(roadJustCreated.transform.position);
            }

        }
        if (previsualisation == false)
            rotationOfTheLastRoadPart += rotationValue;
    }

    IEnumerator DrawNewSegment(float duration)
    {
        for (int i = 0; i < smoothness; i++)
        {
            GameObject roadJustCreated = null;
            if (curveTab.Count != 1)
            {
                roadJustCreated = Instantiate(physicalRoad, GetPoint(curveTab[curveTab.Count - 1], curveTab[curveTab.Count - 2], (i / (float)smoothness)), Quaternion.identity, roadHolder.transform);

                if (bufferRoad == null)
                {
                    bufferRoad = roadJustCreated;
                }
                else
                {
                    bufferRoad.transform.LookAt(roadJustCreated.transform);
                    if (i == smoothness - 1)
                        roadJustCreated.transform.LookAt(bufferRoad.transform);
                    bufferRoad = roadJustCreated;
                }

                yield return new WaitForSeconds(duration);
            }
            else
            {
                yield return null;
            }
        }

    }
}
