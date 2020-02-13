using UnityEngine;
using System.Collections;

public class Shatter : MonoBehaviour {

    public GameObject[] Activater;
    private int ObjCount;

	// Use this for initialization
	void Start () {
        ObjCount = Activater.GetLength(0);
        for (int i = 0; i < ObjCount; i++)
        {
            Activater[i].SetActive(true);
            float power = Random.value*100;
            int direction = (System.Convert.ToInt32(Random.value<0.5))*2-1;

            Activater[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(power * direction, power));
        }
        Destroy(gameObject, 2);
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
	}
}
