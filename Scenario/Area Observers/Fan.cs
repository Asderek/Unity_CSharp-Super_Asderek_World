using UnityEngine;
using System.Collections;

public class Fan : AreaEffector {

    
    public Vector2 intensity = new Vector2(0,20);

    protected override void applyEffect(GameObject visitor)
    {
        visitor.GetComponent<Rigidbody2D>().AddForce(new Vector3(intensity.x, intensity.y, 0));

    }

    protected override void newVisitor(GameObject visitor)
    {

        visitor.GetComponent<Rigidbody2D>().velocity /= 2;
    }

    protected override void exitVisitor(GameObject visitor)
    {

    }

    protected override void onScreenEnter()
    { }
    protected override void onScreenExit()
    { }
}
