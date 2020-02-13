using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ControlledCamera : BaseCamera
{
    


    public float step = 0.1f;
    private Vector2 initPosition;
    private Transform playerTrans;

    protected override void Start()
    {
        base.Start();
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        initPosition = playerTrans.position;

    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        if ((Input.GetAxisRaw("L_VERTICAL") < 0))
        {
            transform.position = new Vector3(transform.position.x,transform.position.y-step,transform.position.z);
        }
        if ((Input.GetAxisRaw("L_VERTICAL") > 0))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + step, transform.position.z);
        }
        if ((Input.GetAxisRaw("L_HORIZONTAL") < 0))
        {
            transform.position = new Vector3(transform.position.x-step, transform.position.y, transform.position.z);
        }
        if ((Input.GetAxisRaw("L_HORIZONTAL") > 0))
        {
            transform.position = new Vector3(transform.position.x + step, transform.position.y, transform.position.z);
        }

        if (Input.GetButton("TRIANGLE"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + step);
        }
        if (Input.GetButton("SQUARE"))
        {
            transform.position = new Vector3(transform.position.x + step, transform.position.y, transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y - step, transform.position.z);
        }
        if (Input.GetButton("CIRCLE"))
        {
            transform.position = new Vector3(transform.position.x - step, transform.position.y, transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y + step, transform.position.z);
        }

        if (Input.GetButton("X"))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - step);
        }

        playerTrans.position = initPosition;

        
    }



}
