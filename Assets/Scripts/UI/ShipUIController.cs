using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShipUIController : MonoBehaviour
{
    public BarController m_EnergyBarController;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(m_EnergyBarController);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEnergyPortion(float portion)
    {
        m_EnergyBarController.SetPortion(portion);
    }
}
