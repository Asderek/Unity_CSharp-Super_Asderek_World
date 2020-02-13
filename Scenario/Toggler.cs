using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Toggler : MonoBehaviour {

    private List<GameObject> objects;
    public float toggleTime;
    private float lastChange;
    private int currentFrame;
    private int direction  =1;


	// Use this for initialization
	void Start () {
       //print((("Size " + GetComponentsInChildren<Transform>().Length);

        objects = new List<GameObject>();

        foreach (Transform t in GetComponentsInChildren<Transform>()) {

            //print((("Obj " + t.gameObject);

            if (t.gameObject.tag == "Animate") {
                objects.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }

        }

       //print((("Count " + objects.Count);

        lastChange = Time.time;
        currentFrame = 1;
        objects[0].SetActive(true);

    }

    // Update is called once per frame
    void Update () {

        if (Time.time - lastChange > toggleTime) {
            lastChange = Time.time;
            objects[currentFrame].SetActive(false);

            //if( (currentFrame == objects.Count -1) || (currentFrame == 0))
            //    direction *= -1;
            //currentFrame += direction;

            currentFrame = (currentFrame + 1) % objects.Count;

            objects[currentFrame].SetActive(true);
        }

	}
}
