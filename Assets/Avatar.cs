using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Avatar : MonoBehaviour
{
    [SerializeField] SteamVR_Action_Boolean triggerAction;
    private List<Shape> m_bubblesInRange = new List<Shape>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if(triggerAction.GetState(SteamVR_Input_Sources.LeftHand))
        {
            Absorbe();
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
