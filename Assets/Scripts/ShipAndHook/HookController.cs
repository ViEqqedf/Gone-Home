using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class HookController : MonoBehaviour
{
    public GameObject m_GOShip;
    public float m_HookSpeed = 1f;
    public float m_HookMaxLen = 10f;

    [ReadOnly]
    public Vector3 m_RelOffsetHook;

    private Ship m_Ship;
    private ShipController m_ShipController;
    private float m_CurProgress = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(m_GOShip);

        m_Ship = m_GOShip.GetComponent<Ship>();
        Assert.IsNotNull(m_Ship);
        m_ShipController = m_GOShip.GetComponent<ShipController>();
        Assert.IsNotNull(m_ShipController);

        m_RelOffsetHook = m_GOShip.transform.worldToLocalMatrix.MultiplyVector(transform.position - m_GOShip.transform.position);
    }

    public void UpdateFollowShipPos()
    {
        Vector3 offset = m_GOShip.transform.localToWorldMatrix.MultiplyVector(m_RelOffsetHook);
        transform.position = m_GOShip.transform.position + offset;
    }

    public void UpdateRot()
    {
        transform.rotation = m_GOShip.transform.rotation;
    }

    public void InitHookStateBeforeShoot()
    {
        m_CurProgress = 0f;
    }

    public void InitHookStateBeforeRetrieve()
    {
        m_CurProgress = (m_GOShip.transform.position - transform.position).magnitude;
    }

    public float GetCurProgress()
    {
        return m_CurProgress;
    }

    public bool UpdateShootHookPos(Vector3 direction)
    {
        bool finished = false;

        if(m_CurProgress< m_HookMaxLen)
        {
            m_CurProgress += m_HookSpeed * Time.deltaTime;
        }
        else
        {
            m_CurProgress = m_HookMaxLen;
            finished = true;
        }

        Vector3 offset = m_GOShip.transform.localToWorldMatrix.MultiplyVector(m_RelOffsetHook);
        transform.position = m_GOShip.transform.position + offset + direction * m_CurProgress;

        return finished;
    }

    public bool UpdateRetrieveHookPos(Vector3 direction)
    {
        bool finished = false;

        if (m_CurProgress > 0)
        {
            m_CurProgress -= m_HookSpeed * Time.deltaTime;
        }
        else
        {
            m_CurProgress = 0;
            finished = true;
        }

        Vector3 offset = m_GOShip.transform.localToWorldMatrix.MultiplyVector(m_RelOffsetHook);
        transform.position = m_GOShip.transform.position + offset + direction * m_CurProgress;

        return finished;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        State curState = m_Ship.m_ShipController.m_StateMachine.CurrentState;
        if (curState.StateId == State.StateIdEnum.StateHookLocked) {
            return;
        }

        Debug.Log($"{transform.name} got collision: {collision.transform.root.name}");

        if (collision.gameObject.layer == LayerMask.NameToLayer("HookColliders"))
        {
            Planet planet = collision.gameObject.transform.parent.GetComponent<Planet>(); // Get planet from the fake collider's parent object
            if (planet != null)
            {
                m_ShipController.OnHookGrabOnPlanet(planet);
            }
        }
    }
}