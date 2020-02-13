using UnityEngine;
using System.Collections;

public class ConveyorBelt : Floor {

    public Vector2 intensity = new Vector2(35, 0);
    public float minSpeedRatio;

	// Update is called once per frame
    //protected override void FixedUpdate () {

    //    foreach (GameObject visitor in visitors)
    //    {
    //        visitor.GetComponent<Rigidbody2D>().AddForce(new Vector3(intensity.x, intensity.y, 0));
    //        if (((((int)visitor.GetComponent<Transform>().eulerAngles.y) / -90) + 1) == (intensity.x / Mathf.Abs(intensity.x)))
    //        {
    //            visitor.GetComponent<Character>().updateSpeed(accelTimeRatio, maxSpeedRatio);
    //        }
    //        else
    //        {
    //            visitor.GetComponent<Character>().updateSpeed(accelTimeRatio, minSpeedRatio);
    //        }
    //    }

    //    //base.FixedUpdate();
    //}
}
