using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStoneCircling : MonoBehaviour {
    public GameObject parent;
    public float m_AngularRotateSpeed;

    private bool isDead = false;

    void Start() {
        isDead = false;
    }

    void Update() {
        if (!isDead) {
            transform.RotateAround(parent.transform.position,
                Vector3.forward, m_AngularRotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Ship ship = collision.GetComponent<Ship>();
        if (ship) {
            isDead = true;
        }
    }
}