using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    [SerializeField] Rigidbody m_rigidbody;
    private int m_index;
    [SerializeField] PositionCurve m_positionCurve;
    private bool go;
    [SerializeField] float m_speed;
    private Vector3 m_lastPosition;
    Vector3 lastDirection = Vector3.zero;
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            go = true;
        }
        if (go)
        {
            Vector3 direction = (m_positionCurve.positions[m_index] - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, direction, 3f*Time.deltaTime);
            m_rigidbody.MovePosition(transform.position + direction * Time.deltaTime*m_speed);
            if (Mathf.Approximately(Vector3.Distance(m_positionCurve.positions[m_index], m_lastPosition),Vector3.Distance(transform.position,m_lastPosition))
                || Vector3.Distance(transform.position, m_lastPosition) > Vector3.Distance(m_positionCurve.positions[m_index], m_lastPosition))
            {
                m_index++;
                m_lastPosition = transform.position;
            }
        }
    }
}
