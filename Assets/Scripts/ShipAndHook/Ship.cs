using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Ship : MonoBehaviour
{
    public ShipController m_ShipController;
    public ShipUIController m_ShipUIController;

    public GameObject m_tailFlameGo;
    public GameObject m_shipExplodeGo;
    public float m_SpeedDrainingPlanetEnergy = 1.0f;
    public float m_SpeedGainingEnergy = 1.0f;
    public float m_SpeedLosingEnergy = 2.0f;
    public float m_SpeedLoseEnergyByTurning = 2.0f;
    public float m_InitEnergyAmount = 0f;
    public float m_TotalEnergyCapacity = 10.0f;

    public bool m_isDead { get; private set; }
    public bool m_isWin { get; private set; }
    public GameObject m_earthGo { get; private set; }

    private float m_CurrentEnergyAmount;
    private bool m_isLaunched = false;
    private float launchTime = 2;
    private float deadCountDown = 3;

    // Start is called before the first frame update
    void Start()
    {
        m_ShipController = GetComponent<ShipController>();
        Assert.IsNotNull(m_ShipController);
        Assert.IsNotNull(m_ShipUIController);

        m_CurrentEnergyAmount = m_InitEnergyAmount;
        SetDead(false);

        m_isLaunched = false;
        m_ShipController.enabled = false;
        m_tailFlameGo.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isDead) {
            deadCountDown -= Time.deltaTime;
            if (deadCountDown < 0) {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GoneHome");
            }
            return;
        }

        if (!m_isLaunched) {
            if (Input.anyKey) {
                m_isLaunched = true;
                m_ShipController.enabled = true;
            }
        } else {
            launchTime -= Time.deltaTime;
            if (launchTime <= 0) {
                m_tailFlameGo.GetComponent<Animation>().Play();
                launchTime = float.MaxValue;
            }
        }

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
            LoseEnergy();
        }

        if(m_CurrentEnergyAmount==0)
        {
            OnEnergyEmpty();
        }

        m_ShipUIController.SetEnergyPortion(m_CurrentEnergyAmount / m_TotalEnergyCapacity);
    }

    public void SetDead(bool isDead) {
        m_isDead = isDead;
        m_ShipController.enabled = !isDead;

        if (isDead) {
            GameObject.Instantiate(m_shipExplodeGo, transform.position, Quaternion.identity);
            for (int i = 0, count = transform.childCount; i < count; i++) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    public void SetWin(GameObject go) {
        m_isWin = true;
        m_earthGo = go;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("HookColliders") ||
            collision.gameObject.CompareTag("FallingStone")) {
            Debug.Log("[ViE] Dead!!!");
            SetDead(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
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

    private void LoseEnergy()
    {
        ConsumeEnergy(Time.deltaTime * m_SpeedLosingEnergy);
    }

    public void ConsumeEnergyByTurning()
    {
        ConsumeEnergy(Time.deltaTime * m_SpeedLoseEnergyByTurning);
    }

    private void ConsumeEnergy(float energy)
    {
        m_CurrentEnergyAmount -= energy;
        if (m_CurrentEnergyAmount < 0)
        {
            m_CurrentEnergyAmount = 0;
        }
    }


    private void OnEnergyEmpty()
    {

    }
}