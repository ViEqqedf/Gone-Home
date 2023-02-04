using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStone : MonoBehaviour {
    public Vector3 m_flyDir;
    public float speed;

    private bool isDead = false;
    private PlanetController planetController;

    void Start() {
        planetController = GetComponent<PlanetController>();
        planetController.m_AngularRotateSpeed = Random.Range(-50, 50);
        isDead = false;
    }

    void Update() {
        if (!isDead) {
            transform.position += m_flyDir.normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Ship ship = collision.GetComponent<Ship>();
        if (ship) {
            isDead = true;
            Destroy(planetController);
        }
    }
}