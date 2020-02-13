using UnityEngine;
using System.Collections;

public class RisingDoor : Door{


    private float finalPosition;
    public float velocity = 0.01f ;
    private float lerp = 1;

    Vector3 openPosition, closedPosition;

    protected override void Start()
    {
        base.Start();
        openPosition = transform.position;
        closedPosition = openPosition;
      
        if (transform.rotation.z != 0)
            openPosition.y += GetComponent<BoxCollider2D>().size.x * transform.localScale.x * transform.parent.transform.localScale.x;
        else
            openPosition.y += GetComponent<BoxCollider2D>().size.y * transform.localScale.y * transform.parent.transform.localScale.y;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Boundaries"), LayerMask.NameToLayer("Boundaries"));

	}

    public override void close()
    {
        lerp += velocity;
        if (lerp > 1)
            lerp = 1f;
        transform.position = Vector3.Lerp(openPosition, closedPosition, lerp);

    }

    public override void  open()
    {
        lerp -= velocity;
        if (lerp <0)
            lerp = 0;
        transform.position = Vector3.Lerp(openPosition, closedPosition, lerp);

    }

}
