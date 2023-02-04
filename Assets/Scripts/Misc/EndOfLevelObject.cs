using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EndOfLevelObject : MonoBehaviour
{
    public Collider2D m_Collider;

    // Start is called before the first frame update
    void Start()
    {
        m_Collider = GetComponent<Collider2D>();
        Assert.IsNotNull(m_Collider);
        Assert.IsTrue(m_Collider.isTrigger);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ship ship = collision.GetComponent<Ship>();
        if (ship)
        {
            //???
            Debug.Log("FUCK Yeahhhhhhh!!");
        }
    }
}
