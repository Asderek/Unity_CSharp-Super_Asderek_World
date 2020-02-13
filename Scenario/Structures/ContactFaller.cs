using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactFaller : SimpleFaller {

    public float maxTimeDelay = 0f;

	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    protected override void HitPlayer(GameObject target)
    {
        //print((("HitPlayer");
        base.HitPlayer(target);
        StartCoroutine(StartFall());
    }

    IEnumerator StartFall()
    {
        yield return new WaitForSeconds(maxTimeDelay);
        GetComponent<Rigidbody2D>().isKinematic = false;
    }


}
