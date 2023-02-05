using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayPositioned : MonoBehaviour {
    public float delayTime;
    private bool startCountDown = false;
    private bool isShowed = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey) {
            startCountDown = true;
        }

        if (!isShowed && startCountDown) {
            delayTime -= Time.deltaTime;
            if (delayTime < 0) {
                transform.position =
                    GameObject.Find("Ship").transform.position + new Vector3(25, -3);
                isShowed = true;
            }
        }
    }
}