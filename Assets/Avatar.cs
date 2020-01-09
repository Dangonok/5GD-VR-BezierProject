using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class Avatar : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Boolean triggerAction;
    [SerializeField] SplineManager splineManager;
    private List<Shape> m_bubblesInRange = new List<Shape>();
    [SerializeField] Text collectibleText;
    int collectibleCount = 0;


    [Header ("Compass Part")]
    public GameObject interestPoint;
    public GameObject interestPointCompassPos;

    // Start is called before the first frame update
    void Start()
    {
        collectibleText.text = "Collectible : " + collectibleCount.ToString();
    }

    void Update()
    {
        PositionOfThePointOfInterest();

        if(triggerAction.GetState(SteamVR_Input_Sources.LeftHand))
        {
            Absorbe();
        }

    }

    void PositionOfThePointOfInterest()
    {
        interestPointCompassPos.transform.LookAt(interestPoint.transform);
        float distanceBetweenAvatarAndInterest = Vector3.Distance(this.transform.position, interestPoint.transform.position);
        if (distanceBetweenAvatarAndInterest < GameManager.Instance.datas.maxDistanceCompass)
        {
            interestPointCompassPos.transform.localPosition = Vector3.zero + transform.forward * 
                (distanceBetweenAvatarAndInterest / GameManager.Instance.datas.maxDistanceCompass)* 0.3f;
        }
        else
        {
            interestPointCompassPos.transform.localPosition = Vector3.zero + transform.forward*0.3f;
        }
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
            collectibleText.text = "Collectible : " + collectibleCount.ToString();
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
