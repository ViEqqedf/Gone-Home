using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EndOfLevelObject : MonoBehaviour
{
    public int m_curLevel;
    public GameObject m_congratulation;
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
            StartCoroutine(ShowCongratulation());
            Debug.Log("FUCK Yeahhhhhhh!!");
            // temp
            ship.SetDead(true);
        }
    }

    private IEnumerator ShowCongratulation()
    {
        if (m_congratulation != null) {
            m_congratulation.SetActive(true);
        }
        yield return new WaitForSeconds(2);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level" + (m_curLevel + 1));
    }
}