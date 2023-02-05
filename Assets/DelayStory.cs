using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DelayStory : MonoBehaviour {
    public float delayTime;
    public bool destroyWhenGameStart;
    public bool delayAfterGameStart;
    private bool isShowed;
    private GameObject shipGo;

    // Start is called before the first frame update
    void Start()
    {
        shipGo = GameObject.Find("Ship");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey) {
            if (destroyWhenGameStart && Input.anyKey) {
                Destroy(this.gameObject);
            } else if (delayAfterGameStart) {
                delayAfterGameStart = false;
            }
        }

        if (!isShowed) {
            transform.position = shipGo.transform.position + 2 * Vector3.up;
            if (!delayAfterGameStart) {
                delayTime -= Time.deltaTime;
            }
            if (delayTime < 0) {
                GetComponent<Animation>().Play();
                isShowed = true;
            }
        }
    }
}