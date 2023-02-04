using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [Header("This is degree! ")]
    public float m_AngularRotateSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RotatePlanet();
    }

    void RotatePlanet()
    {
        transform.rotation = (Quaternion.Euler(0, 0, m_AngularRotateSpeed * Time.deltaTime) * transform.rotation).normalized;
    }
}