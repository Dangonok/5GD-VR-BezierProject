using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using DG.Tweening;

public class Avatar : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Boolean triggerAction;
    [SerializeField] SplineManager splineManager;
    private List<Shape> m_bubblesInRange = new List<Shape>();
    [SerializeField] Text collectibleText;
    int collectibleCount = 0;
    int lastCollectibleCount = 0;
    [SerializeField] int howManyCollectibleYouNeed;


    [Header ("Compass Part")]
    public GameObject interestPoint;
    public GameObject interestPointCompassPos;
    public Transform avatarCompass;

    Vector3 posToLookAt;

    // Start is called before the first frame update
    void Start()
    {
        collectibleText.text =  "Collectible : " + collectibleCount.ToString() + "/" + howManyCollectibleYouNeed.ToString();
        PositionOfThePointOfInterest();
        interestPointCompassPos.transform.LookAt(posToLookAt);
        GlitchedCompass();
       // StartCoroutine(GlitchedCompass());
    }



    void GlitchedCompass()
    {
        Quaternion from = interestPointCompassPos.transform.rotation;
        interestPointCompassPos.transform.LookAt(posToLookAt);
        Quaternion goodRotation = interestPointCompassPos.transform.rotation;
        Quaternion to = Quaternion.Slerp(interestPointCompassPos.transform.rotation, Random.rotation, 0.25f);
        interestPointCompassPos.transform.rotation = from;
        if (collectibleCount != howManyCollectibleYouNeed)
        {
            interestPointCompassPos.transform.DORotateQuaternion(to, 3f).SetEase(Ease.InOutBounce).onComplete += GlitchedCompass;
        }
        else
        {
            interestPointCompassPos.transform.DORotateQuaternion(goodRotation, 1f).SetEase(Ease.Linear).onComplete += GlitchedCompass;
        }
    }

    void Update()
    {


        if (collectibleCount > lastCollectibleCount)
        {
            PositionOfThePointOfInterest();
            lastCollectibleCount = collectibleCount;
        }



        if(triggerAction.GetState(SteamVR_Input_Sources.LeftHand))
        {
            Absorbe();
        }

    }

    void PositionOfThePointOfInterest()
    {
        //print("0");
        //interestPointCompassPos.transform.LookAt(interestPoint.transform);
        //float distanceBetweenAvatarAndInterest = Vector3.Distance(this.transform.position, interestPoint.transform.position);
        //if (distanceBetweenAvatarAndInterest < GameManager.Instance.datas.maxDistanceCompass)
        //{
        //    print("1");
        //    interestPointCompassPos.transform.GetChild(0).transform.localPosition = Vector3.forward * 
        //        (distanceBetweenAvatarAndInterest / GameManager.Instance.datas.maxDistanceCompass)*0.45f;
        //}
        //else
        //{
        //    print("2");
        //    interestPointCompassPos.transform.GetChild(0).transform.localPosition = Vector3.forward*0.45f;
        //}
        posToLookAt = Random.onUnitSphere * (GameManager.Instance.datas.maxSphereRange - GameManager.Instance.datas.maxSphereRange * (collectibleCount / howManyCollectibleYouNeed)) + interestPoint.transform.position;
    }

    private void Absorbe()
    {
        for (int i = m_bubblesInRange.Count-1; i >= 0; i--)
        {
            print("processing bubble " + m_bubblesInRange[i].gameObject.name);
            Shape thisBubble = m_bubblesInRange[i];
            thisBubble.transform.localScale = Vector3.MoveTowards(thisBubble.transform.localScale, Vector3.zero, Time.deltaTime*150f/4);
            if (thisBubble.transform.localScale.sqrMagnitude <= 0.2f*0.2f)
            {
                m_bubblesInRange.RemoveAt(i);
                Destroy(thisBubble.gameObject);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Death")
        {
            print("death");
            splineManager.DestroyTheLastX(GameManager.Instance.datas.deathRedo);
        }
        if (other.tag == "collectible")
        {
            collectibleCount += 1;
            collectibleText.text = "Collectible : " + collectibleCount.ToString() + "/" + howManyCollectibleYouNeed.ToString();
            Destroy(other.gameObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.CompareTag("Bubble"))
    //    {
    //        Shape thisBubble = other.GetComponent<Shape>();
    //        if(!m_bubblesInRange.Contains(thisBubble))
    //        {
    //            m_bubblesInRange.Add(thisBubble);
    //            thisBubble.m_meshRenderer.material.color = Color.red;
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Bubble"))
    //    {
    //        Shape thisBubble = other.GetComponent<Shape>();
    //        if (m_bubblesInRange.Contains(thisBubble))
    //        {
    //            m_bubblesInRange.Remove(thisBubble);
    //            thisBubble.m_meshRenderer.material.color = Color.white;
    //        }
    //    }
    //}
}
