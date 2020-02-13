using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSwitch : Switch {

    public float activateCD;
    private float lastActivate;

    public int numberOfStates;
    private int i = 0;

    protected override void OnTriggerStay2D(Collider2D colisor)
    {
    }

    //void Update()
    //{
    //    //print(((gameObject + " -> State1 = " + GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("state1"));
    //    //print(((gameObject + " -> State2 = " + GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("state2"));
    //    //print(((gameObject + " -> State3 = " + GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("state3"));
    //    //print(((gameObject + " -> State4 = " + GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("state4"));
    //    //print(((gameObject + " -> State5 = " + GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("state5  "));

    //}

    protected override void Start()
    {
        base.Start();
        status = Status.Off;
    }

    protected virtual void OnTriggerEnter2D(Collider2D colisor)
    {
        if (Time.time - lastActivate > activateCD)
        {
            if (Assets.Scripts.Utilities.HitAsderekAttack(colisor))
            {
                if (enabled == true)
                {
                    status = Status.Off;
                    ActivateInteraction();
                    i++;
                    if (i > numberOfStates - 1)
                    {
                        Reset();
                        i = 0;
                    }

                    lastActivate = Time.time;

                }
            }
        }

    }

}
