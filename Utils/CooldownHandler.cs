using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CooldownHandler {

    public float desiredCD;
    public bool isTimer;
    private float lastCD;

	// Use this for initialization
    public void init()
    {
        lastCD = -desiredCD;
	}
	
	// Update is called once per frame
    public void update()
    {
        if (!isTimer)
            lastCD--;
	}

    public void startCD()
    {
        if (isTimer)
            lastCD = Time.time;
        else
            lastCD = desiredCD;
    }

    public bool isOnCD()
    {
        if (isTimer)
            return Time.time - lastCD <= desiredCD;
        else
            return lastCD != 0;
    }

    public void changeTimer(float time)
    {
        desiredCD = time;
    }

    public float remainingTime()
    {
        if (isTimer)
            return (Time.time - lastCD)/desiredCD;
        else
            return lastCD/desiredCD;
    }

}
