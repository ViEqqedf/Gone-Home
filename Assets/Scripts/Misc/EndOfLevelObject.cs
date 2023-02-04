using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EndOfLevelObject : MonoBehaviour
{
    public int m_curLevel;
    public Collider2D m_Collider;
    public GameObject m_welcomeGo;

    // Start is called before the first frame update
    void Start()
    {
        m_Collider = GetComponent<Collider2D>();
        Assert.IsNotNull(m_Collider);
        Assert.IsNotNull(m_welcomeGo);
        Assert.IsTrue(m_Collider.isTrigger);
        m_welcomeGo.SetActive(false);
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
            // StartCoroutine(ShowCongratulation());
            Debug.Log("FUCK Yeahhhhhhh!!");
            m_welcomeGo.SetActive(true);
            ship.SetWin(transform.gameObject);
        }
    }

    private IEnumerator ShowCongratulation()
    {
        yield return new WaitForSeconds(2);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level" + (m_curLevel + 1));
    }
}