using UnityEngine;
using System.Collections;

public class NureOnna : MonoBehaviour {

    public float value;
    public float upvalue;
    private Vector2 startPosition;
	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        //GetComponent<Rigidbody2D>().velocity = new Vector2(0,value);

	}
	
	// Update is called once per frame
	void Update () {
        Vector2 newPosition;
        if (transform.position.y > startPosition.y + upvalue)
        {
            value = -value;
            float direction = 180 - transform.eulerAngles.z;
            transform.eulerAngles = new Vector3(0, 0, direction);
        }
        
        if (transform.position.y < startPosition.y - 2)
        {
            value = -value;
            float direction = 180 - transform.eulerAngles.z;
            transform.eulerAngles = new Vector3(0, 0, direction);
        }
        newPosition = new Vector2(transform.position.x,transform.position.y + value);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.time);
	}
}
