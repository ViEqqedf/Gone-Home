using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStoneCircling : MonoBehaviour {
    public GameObject parent;
    public float m_AngularRotateSpeed;

    private bool isLaunched = false;
    private bool isDead = false;
    private GameObject ship;

    void Start() {
        isDead = false;
        ship = GameObject.Find("Ship");
    }

    void Update() {
        if (!isLaunched && transform.position.x - ship.transform.position.x < 25) {
            isLaunched = true;
        }

        if (isLaunched && !isDead) {
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