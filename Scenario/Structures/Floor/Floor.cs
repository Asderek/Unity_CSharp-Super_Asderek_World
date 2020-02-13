using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Floor : MonoBehaviour {

    private Collider2D targetCollider;
    private Vector2[] myPoints;
    private Vector2 leftPoint, rightPoint;
    public float considerCollisionCD = 1f;
    private float lastTime;
    public float accelTimeRatio=1f;
    public float maxSpeedRatio=1f;
    private Bounds myBounds;

    [HideInInspector]
    public bool isPassable = false;
    //public 

    protected List<GameObject> visitors;

    public struct Boundaries
    {
        public Vector2 leftPoint;
        public Vector2 rightPoint;
    }

	// Use this for initialization
	protected virtual void Start () {

        visitors = new List<GameObject>();

        if (gameObject.tag == "Passable")
        {
            isPassable = true;
        }

        targetCollider = null;

        gameObject.tag = "Floor";
        gameObject.layer = LayerMask.NameToLayer("Floor");

        foreach(Collider2D col in gameObject.GetComponents<Collider2D>())
        {
            if (col.isTrigger == false && col.enabled)
            {
                if (col is BoxCollider2D)
                {
                    BoxCollider2D myCollider = (BoxCollider2D)col;
                    Vector2 floorSize = myCollider.size;

                    float rotation = transform.rotation.eulerAngles.z;

                    Vector2 floorScale = new Vector2(transform.localScale.x * transform.parent.transform.lossyScale.x, transform.lossyScale.y * transform.parent.transform.lossyScale.y);


                    leftPoint.y = transform.position.y + Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * floorScale.y - Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * floorScale.x;
                    leftPoint.x = transform.position.x - Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * floorScale.y - Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * floorScale.x;

                    rightPoint.y = transform.position.y + Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * floorScale.y + Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * floorScale.x;
                    rightPoint.x = transform.position.x - Mathf.Sin(rotation * Mathf.Deg2Rad) * floorSize.y / 2 * floorScale.y + Mathf.Cos(rotation * Mathf.Deg2Rad) * floorSize.x / 2 * floorScale.x;
                }
                else if (col is EdgeCollider2D)
                {
                    EdgeCollider2D myCollider = (EdgeCollider2D)col;


                    Vector2[] points = myCollider.points;
                    Vector2 offSet = myCollider.offset;
                    myPoints = points;


                    leftPoint = Vector2.Scale((points[0] + offSet), transform.lossyScale) + (Vector2)transform.position;
                    rightPoint = Vector2.Scale((points[points.Length-1] + offSet), transform.lossyScale) + (Vector2)transform.position;

                }


                col.usedByEffector = true;
                PlatformEffector2D effector = gameObject.AddComponent<PlatformEffector2D>();
                if (effector)
                {
                    effector.useOneWay = false;
                    effector.colliderMask = effector.colliderMask & ~(1 << 12);
                }

                if (isPassable)
                {
                    if (effector)
                    {
                        effector.useOneWay = true;
                        effector.surfaceArc = 100f;
                    }
                }

                myBounds = col.bounds;
                break;
            }
        }
       
	}

    public float GetRotation(float x)
    {
        for (int i = 1; i < myPoints.Length; i++)
        {
            if (((x > myPoints[i - 1].x) && (x < myPoints[i].x)) || ((x < myPoints[i - 1].x) && (x > myPoints[i].x)))
            {
                Vector2 rotation = myPoints[i] - myPoints[i - 1];
                return Mathf.Atan2(rotation.y, rotation.x)*Mathf.Rad2Deg;
            }
        }


        return 0;
    }

    protected virtual void FixedUpdate()
    {
        if (targetCollider != null)
        {
            Vector3 size;
            size = Vector3.Scale(myBounds.size, transform.parent.transform.lossyScale);

            if (Time.time - lastTime < considerCollisionCD)
                return;

            foreach (Collider2D a in GetComponents<Collider2D>())
            {
                if (a.isTrigger == false)
                    Physics2D.IgnoreCollision(a, targetCollider, false);

            }
            targetCollider = null;

            return;
        }
    }
	
    protected virtual void OnCollisionEnter2D(Collision2D colidido)
    {
            visitors.Add(colidido.gameObject);     
    }

    protected virtual void OnCollisionExit2D(Collision2D colidido)
    {
            visitors.Remove(colidido.gameObject);
    }

    public virtual void ignoreColision(BoxCollider2D target, bool state)
    {
        targetCollider = target;

        foreach (Collider2D a in GetComponents<Collider2D>())
        {
            if (a.isTrigger == false)
                Physics2D.IgnoreCollision(a, targetCollider, state);
 
        }
        
        lastTime = Time.time;
    }

    public virtual float GetYPoint(float x)
    {
        Vector2 destinyDirection = rightPoint - leftPoint;
        destinyDirection.Normalize();

        return (leftPoint.y + (x - leftPoint.x) * destinyDirection.y / destinyDirection.x);
    }

    public Boundaries GetLedge()
    {
        Boundaries boundaries;
        boundaries.leftPoint = leftPoint;
        boundaries.rightPoint = rightPoint;

        return boundaries;
    }
}
