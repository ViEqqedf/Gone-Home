using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    public GameObject m_ToFollow;

    private Vector3 m_RefOffset;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(m_ToFollow);
        m_RefOffset = transform.position - m_ToFollow.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_ToFollow.transform.position + m_RefOffset;
    }
}