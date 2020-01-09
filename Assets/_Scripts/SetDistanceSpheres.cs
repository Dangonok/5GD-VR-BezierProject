using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SetDistanceSpheres : MonoBehaviour
{
    [SerializeField] int m_numberOfSphere;
    [SerializeField] GameObject m_object;
    [SerializeField] Transform m_transformPlayer;
    [SerializeField] int m_speherRadius;
    [SerializeField] Material m_mat;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_numberOfSphere; i++)
        {
            Vector3 pos = Random.insideUnitSphere * m_speherRadius;
            int rScale = Random.Range(GameManager.Instance.datas.spheresMinRadius, GameManager.Instance.datas.spheresMaxRadius);
            GameObject go = Instantiate(m_object, pos, Quaternion.identity) as GameObject;
            go.transform.localScale = new Vector3(rScale, rScale, rScale);
            go.transform.LookAt(m_transformPlayer);
            go.transform.parent = transform;
            // go.transform.GetChild(0).GetComponent<TextMeshPro>().text = i.ToString() ;
            go.name = "sphere-"+i.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
