using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnee : MonoBehaviour {

    public delegate void TickHandler(float time);
    public event TickHandler Tick;
    public float waitTime = 1;
    
    void OnDestroy()
    {
        ActivateTick(waitTime);
    }
    
    protected void ActivateTick(float waitTime)
    {
        if(Tick != null)
            Tick(waitTime);
    }
}
