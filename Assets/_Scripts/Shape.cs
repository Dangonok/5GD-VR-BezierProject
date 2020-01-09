using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shape : MonoBehaviour
{
    private bool m_isTuched;
    public MeshRenderer m_meshRenderer;
    [SerializeField] GameObject m_particle;
    [SerializeField] float m_timeToLight;
    [SerializeField] float m_timeToShutLight;

    public enum ShapeState
    {
        Idle,
        Tuched
    }

    private ShapeState m_state;

    public ShapeState State
    {
        get
        {
            return m_state;
        }
        set
        {
            if (value != m_state)
            {
                float startRed = m_meshRenderer.material.color.r;
                m_meshRenderer.material.DOKill();
                switch (value)
                {
                    case ShapeState.Idle:
                        m_meshRenderer.material.DOColor(Color.white, 1 - m_timeToShutLight * startRed);
                        break;
                    case ShapeState.Tuched:
                        m_meshRenderer.material.DOColor(Color.red, m_timeToLight * startRed).onComplete += Explosion;
                        break;
                }
                m_state = value;
            }
        }
    }

    private void Start()
    {
        int indexRandom = Random.Range(0, GameManager.Instance.datas.objects.Length);
        GameObject go = Instantiate(GameManager.Instance.datas.objects[indexRandom], transform.position, Quaternion.identity);
        go.transform.parent = transform;
        float scale = GameManager.Instance.datas.spheresMinRadius / 5f;
        go.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void Explosion()
    {
        Instantiate(m_particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            State = ShapeState.Tuched; 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            State = ShapeState.Idle;
    }
}
