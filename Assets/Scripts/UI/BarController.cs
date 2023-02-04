using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    public Image m_Bar;
    public Image m_BarContent;

    private float m_LenOrg;
    private Vector3 m_PosLeftEnd;

    void Awake()
    {
        m_LenOrg = m_BarContent.rectTransform.rect.width;
        m_PosLeftEnd = m_BarContent.rectTransform.localPosition - new Vector3(m_LenOrg / 2, 0, 0);
    }

    public void SetPortion(float portion)
    {
        float len = portion * m_LenOrg;
        //Rect rect = m_BarContent.rectTransform.rect;
        Vector3 pos = m_PosLeftEnd;
        pos.x += len / 2.0f;
        m_BarContent.rectTransform.localPosition = pos;
        m_BarContent.rectTransform.localScale = new Vector3(portion, 1, 1);
    }
}
