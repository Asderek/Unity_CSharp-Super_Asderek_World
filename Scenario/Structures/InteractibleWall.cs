using UnityEngine;
using System.Collections;

public class InteractibleWall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D colision)
    {


        if (colision.gameObject.GetComponent<Asderek>() != null)
        {
            gameObject.SetActive(false);
        }
    }
}
