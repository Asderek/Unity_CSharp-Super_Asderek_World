using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour {

    public float radiusFactor    = 0.5f;
    public float radius = 1f;
    public float maxRadiusSize=3f;
    
    public float ThetaScale = 0.01f;
    private float startRadius;
    private bool parent = true;
    private int Size;
    private LineRenderer LineDrawer;
    private float Theta = 0f;

	// Use this for initialization
	void Start () {
        startRadius = radius;
        LineDrawer = GetComponent<LineRenderer>();
        if (parent)
        {
            parent = false;
            LineDrawer.startWidth = LineDrawer.endWidth = LineDrawer.startWidth / 7;
            Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<Sonar>().parent = false;
            LineDrawer.startWidth = LineDrawer.endWidth = LineDrawer.startWidth * 7;
            radius = 2 * radiusFactor;
            LineDrawer.startWidth = LineDrawer.endWidth = LineDrawer.startWidth / 2;
            Instantiate(gameObject, transform.position, Quaternion.identity).GetComponent<Sonar>().parent = false;
            LineDrawer.startWidth = LineDrawer.endWidth = LineDrawer.startWidth * 2;
            radius = 4 * radiusFactor;

        }
	}
	
	// Update is called once per frame
	void Update () {
 
        Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        LineDrawer.SetVertexCount(Size); 
        for(int i = 0; i < Size; i++){          
            Theta += (2.0f * Mathf.PI * ThetaScale);         
            float x = radius * Mathf.Cos(Theta) + transform.position.x;
            float y = radius * Mathf.Sin(Theta) + transform.position.y;          
            LineDrawer.SetPosition(i, new Vector3(x, y, 0));
        }
    

        if (radius > maxRadiusSize)
        {
            Destroy(gameObject);
            //radiusFactor = 0;
        }
        else
        {
            radius += radiusFactor ;
            GetComponent<CircleCollider2D>().radius = radius;
        }
	}

}
