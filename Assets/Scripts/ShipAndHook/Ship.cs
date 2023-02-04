using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Ship : MonoBehaviour
{
    public ShipController m_ShipController;
    public ShipUIController m_ShipUIController;

    public float m_SpeedDrainingPlanetEnergy = 1.0f;
    public float m_SpeedGainingEnergy = 1.0f;
    public float m_SpeedLosingEnergy = 2.0f;

    public float m_InitEnergyAmount = 5.0f;
    public float m_TotalEnergyCapacity = 10.0f;

    private float m_CurrentEnergyAmount;

    // Start is called before the first frame update
    void Start()
    {
        m_ShipController = GetComponent<ShipController>();
        Assert.IsNotNull(m_ShipController);
        Assert.IsNotNull(m_ShipUIController);

        m_CurrentEnergyAmount = m_InitEnergyAmount;
    }

    // Update is called once per frame
    void Update()
    {
        Planet planet = m_ShipController.GetHookGrabbedPlanet();
        if (planet)
        {
            if (DrainPlanetEnergy(planet)) // Energy all drained
            {
                m_ShipController.TriggerHookRelease();
            }
        }
        else
        {
            if(LoseEnergy())
            {
                OnEnergyEmpty();
            }
        }

        m_ShipUIController.SetEnergyPortion(m_CurrentEnergyAmount / m_TotalEnergyCapacity);
    }

    private bool DrainPlanetEnergy(Planet planet)
    {
        float energyGained = Time.deltaTime * m_SpeedDrainingPlanetEnergy;
        bool drained = planet.DrainEnergy(ref energyGained);

        m_CurrentEnergyAmount += energyGained;
        if(m_CurrentEnergyAmount> m_TotalEnergyCapacity)
        {
            m_CurrentEnergyAmount = m_TotalEnergyCapacity;
        }

        return drained;
    }

    private bool LoseEnergy()
    {
        m_CurrentEnergyAmount -= Time.deltaTime * m_SpeedLosingEnergy;
        if(m_CurrentEnergyAmount<0)
        {
            m_CurrentEnergyAmount = 0;
            return true;
        }
        return false;
    }

    private void OnEnergyEmpty()
    {

    }
}
