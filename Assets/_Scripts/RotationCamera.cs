using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCamera : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y
    };
    public Axis axis = Axis.Y;
    [SerializeField] float m_sensitive;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(axis == Axis.Y) transform.Rotate(0, Input.GetAxis("Mouse X") * Time.deltaTime * m_sensitive, 0);
        else transform.Rotate(- Input.GetAxis("Mouse Y") * Time.deltaTime * m_sensitive, 0, 0);
    }
}
