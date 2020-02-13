using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinderFaller : BreakableFaller {

    public GameObject present;

    [Range (0f,1f)]
    public float percent;
    public Vector3 offset;

    private bool alreadyCreated = false;

	void Update () {
        base.Update();

        if (alreadyCreated)
            return;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("dying") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > percent)
        {
            Instantiate(present, transform.position + offset, Quaternion.identity);
            alreadyCreated = true;
        }

	}

}
