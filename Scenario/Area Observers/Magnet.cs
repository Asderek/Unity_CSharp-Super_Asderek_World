using UnityEngine;
using System.Collections;

public class Magnet : AreaEffector {


    public float intensity = 2f;

    protected override void applyEffect(GameObject visitor) {

        Vector3 direction = (transform.position - visitor.transform.position).normalized;
        visitor.GetComponent<Rigidbody2D>().AddForce( new Vector3(direction.x, direction.y, 0) * intensity);

    }
    
    protected override void newVisitor(GameObject visitor) {

        visitor.GetComponent<Rigidbody2D>().velocity /= 2;
    }
    
    protected override void exitVisitor(GameObject visitor) { 
    
    }

    protected override void onScreenEnter()
    { }
    protected override void onScreenExit()
    { }
}
