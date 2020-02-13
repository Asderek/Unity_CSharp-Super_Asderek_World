using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(LineRenderer))]
public class RouteFloor : EventObserver {

    public GameObject route;
    protected List<Transform> steps;

    protected LineRenderer line;

    protected bool direction = true;
    protected int currentStep = 0;
    private float count = 0;
    protected Vector3 lastPosition;


    public float cornerCD;
    private float lastCornerCD;

    public float velocity = 0.1f;
    public bool on = true;

	// Use this for initialization
	protected virtual void Start () {
        base.Start();
        steps = new List<Transform>(route.GetComponentsInChildren<Transform>());
        steps.Remove(route.transform);

        transform.position = steps[0].position;
        //print(((steps.Count);
        lastPosition = transform.position;
        line = GetComponent<LineRenderer>();
        DrawSteps(steps);

        lastCornerCD = -cornerCD;

        if (observables.Count % 2 != 0)
            on = !on;
	}


	// Update is called once per frame
	protected virtual void FixedUpdate () {

        if (Time.time - lastCornerCD < cornerCD)
            return;

        if (!on)
            return;

        Vector3 newPos = transform.position + (steps[currentStep].position - lastPosition).normalized * velocity;
        newPos.z = transform.position.z;
        transform.position = newPos;

        if ((Mathf.Abs(steps[currentStep].position.x - transform.position.x) < 0.1f) && Mathf.Abs(steps[currentStep].position.y - transform.position.y) < 0.1f)
        {
            //print((("oi");
            HandlePoint(steps[currentStep]);
            if (direction)
            {
                currentStep++;
                if (currentStep >= steps.Count) {
                    HandleEdge();
                }
            }
            else
            {
                currentStep--; 
                if (currentStep < 0)
                {
                    HandleEdge();
                }
            }
            lastPosition = transform.position;

        }
	}

    protected virtual void HandlePoint(Transform t)
    {
    }

    protected virtual void HandleEdge()
    {
        direction = !direction;
        if (currentStep <= 0)
        {
            currentStep = 0;
        }
        else
        {
            currentStep = steps.Count - 1;
        }

        lastCornerCD = Time.time;
    }

    protected virtual void DrawSteps(List<Transform> positions)
    {
        if (line == null)
            return;
        
        int i =0 ;
        line.positionCount = positions.Count;
        foreach( Transform t in positions)
        {
            line.SetPosition(i, t.position);
            i++;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject obj = coll.gameObject;
        while (obj.transform.parent != null) { 
            obj = obj.transform.parent.gameObject;
        }
        obj.transform.parent = transform;
    }

    protected virtual void OnCollisionExit2D(Collision2D coll)
    {
        GameObject obj = coll.gameObject;
        while ( obj.transform.parent != transform && (obj.transform.parent != null))
        {
            obj = obj.transform.parent.gameObject;
        }
        obj.transform.parent = null;

    }


    public override void SendNotification(bool status, Switch source)
    {
        base.SendNotification(status, source);
        this.on = !this.on;
        //print((("Receive notification");
    }
}
