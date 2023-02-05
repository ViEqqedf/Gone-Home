using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CircleCollider2D))]
public class Planet : MonoBehaviour
{
    public PlanetController m_PlanetController;

    public float m_PlanetInitEnergy = 10.0f;
    public float m_PlanetMinScale = 0.1f;

    public List<Object> m_planetImageList = new List<Object>();

    [ReadOnly]
    private float m_PlanetCurEnergy;
    private Vector3 m_PlanetOrgScale;
    private int m_SetToDestroyAtNextNFrame = -1;

    // Start is called before the first frame update
    void Start()
    {
        m_PlanetController = GetComponent<PlanetController>();
        Assert.IsNotNull(m_PlanetController);

        m_PlanetCurEnergy = m_PlanetInitEnergy;
        m_PlanetOrgScale = transform.localScale;

        SpriteRenderer sr = transform.Find("HookCollider").GetComponent<SpriteRenderer>();
        int index = Random.Range(0, m_planetImageList.Count - 1);
        Sprite sprite = Sprite.Create((Texture2D) m_planetImageList[index],
                sr.sprite.textureRect, new Vector2(0.5f, 0.5f));
        sr.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_SetToDestroyAtNextNFrame > 0)
        {
            m_SetToDestroyAtNextNFrame--;
        }
        else if(m_SetToDestroyAtNextNFrame == 0)
        {
            Destroy(gameObject);
        }
    }

    public bool IsClockwiseRotate() {
        return m_PlanetController.m_AngularRotateSpeed < 0;
    }

    public void SetToDestroy()
    {
        m_SetToDestroyAtNextNFrame = 2;
    }

    public bool DrainEnergy(ref float energyLoss)
    {
        m_PlanetCurEnergy -= energyLoss;
        OnDrainEnergy();

        if (m_PlanetCurEnergy <= 0)
        {
            energyLoss += m_PlanetCurEnergy;
            m_PlanetCurEnergy = 0;
            OnEnergyDrained();

            return true;
        }

        return false;
    }

    public void OnDrainEnergy()
    {
        float portion = m_PlanetCurEnergy / m_PlanetInitEnergy;
        float scale = Mathf.Lerp(m_PlanetMinScale, 1.0f, portion);
        transform.localScale = m_PlanetOrgScale * scale;
    }

    public void OnEnergyDrained()
    {

    }
}