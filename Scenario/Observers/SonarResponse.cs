using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarResponse : MonoBehaviour
{

    public float radiusFactor = 0.5f;
    public float radius = 1f;
    public float maxRadiusSize = 3f;
    public GameObject greenBall;

    public float ThetaScale = 0.01f;
    private float startRadius;
    private int size;
    private LineRenderer LineDrawer;
    private float Theta = 0f;
    public bool  parent = false;

    public bool receivePing = false;
    public float echoDirection = 0;
    public float echoAngle = 60;

    static string applyString = "InstantiateEcho";

    // Use this for initialization
    void Start()
    {
        startRadius = radius;
        LineDrawer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!receivePing)
            return;


        size = (int)((1f / ThetaScale) + 1f);

        

        float startAngle = (echoDirection - echoAngle / 2f) %360;
        float endAngle = (echoDirection + echoAngle / 2f) %360;

        startAngle += (startAngle < 0) ? 360 : 0;
        endAngle += (startAngle < 0) ? 360 : 0;

        int startIndex = (int)((startAngle / 360f) * size);
        int finalIndex = (int)((endAngle / 360f) * size);

        if (finalIndex > startIndex)
        {
            size = finalIndex - startIndex + 1;
        }
        else
        {
            size = size - startIndex + finalIndex;
        }

        LineDrawer.positionCount = size;

        Theta = startIndex * (2.0f * Mathf.PI * ThetaScale);
        for (int i = 0; i < size; i++)
        {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = radius * Mathf.Cos(Theta) + transform.position.x;
            float y = radius * Mathf.Sin(Theta) + transform.position.y;
            LineDrawer.SetPosition(i, new Vector3(x, y, 0));
        }


        if (radius > maxRadiusSize)
        {
            if (parent)
            {
                radius = startRadius;
                receivePing = false;
                parent = false;
                GetComponent<CircleCollider2D>().enabled = true;
                LineDrawer.positionCount = 0;
                GameObject son = Instantiate(gameObject, transform.position, Quaternion.identity);
                son.transform.localScale = transform.lossyScale;
                son.transform.parent = transform.parent;
            }
            ApplyOnce.remove(applyString, gameObject);
            Destroy(gameObject);
        }
        else
        {
            radius += radiusFactor;
        }
    }

    void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.GetComponent<Sonar>() != null)
        {      
            ApplyOnce.apply(applyString,gameObject, () =>
            {

                LineDrawer.startColor = colisor.gameObject.GetComponent<LineRenderer>().startColor;
                LineDrawer.endColor = colisor.gameObject.GetComponent<LineRenderer>().endColor;


                Vector2 aux = colisor.transform.position - transform.position;

                echoDirection = Mathf.Rad2Deg*Mathf.Atan2(aux.y, aux.x);

                receivePing = true;

                GetComponent<CircleCollider2D>().enabled = false;




                LineDrawer.startWidth = LineDrawer.endWidth = LineDrawer.startWidth / 7;
                Instantiate(gameObject, transform.position, Quaternion.identity); 
                LineDrawer.startWidth = LineDrawer.endWidth = LineDrawer.startWidth * 7;
                radius = 2 * radiusFactor;
                LineDrawer.startWidth = LineDrawer.endWidth = LineDrawer.startWidth / 2;
                Instantiate(gameObject, transform.position, Quaternion.identity); 
                LineDrawer.startWidth = LineDrawer.endWidth = LineDrawer.startWidth * 2;
                radius = 4 * radiusFactor;

                parent = true;


                return true;
            });
        }
    }

}