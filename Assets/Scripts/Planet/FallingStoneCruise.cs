using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStoneCruise : MonoBehaviour {
    public Vector3 m_flyDir;
    public float speed;

    private bool isLaunched = false;
    private bool isDead = false;
    private PlanetController planetController;
    private GameObject ship;

    void Start() {
        planetController = GetComponent<PlanetController>();
        planetController.m_AngularRotateSpeed = Random.Range(-50, 50);
        isDead = false;
        ship = GameObject.Find("Ship");
    }

    void Update() {
        if (!isLaunched && transform.position.x - ship.transform.position.x < 25) {
            isLaunched = true;
        }

        if (isLaunched && !isDead) {
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