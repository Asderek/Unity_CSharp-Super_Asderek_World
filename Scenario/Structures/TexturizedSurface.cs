using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class TexturizedSurface : MonoBehaviour
{
    protected Vector3 size;
    public float tillingFactor = 1f;
    public bool oneFloor = false;

    public bool animated = false;
    public float range = 2;
    public float velocity = 0.1f;
    private float currentOffset = 0;

    private Material material;
    private Vector2 startOffset;

    protected virtual void Update()
    {
        material = GetComponent<Renderer>().material;
        Texture texture = material.mainTexture;
        size = transform.lossyScale;
        size.x /= tillingFactor;
        size.y /= ((float)texture.height / (float)texture.width) * tillingFactor;

        if (oneFloor)
        {
            size.x /= size.y;
            size.y = 1;
        }
        material.mainTextureScale = size;
        startOffset = material.mainTextureOffset;
    //}


    //protected virtual void Update()
    //{
        if (!animated)
            return;

        currentOffset += velocity;
        if ((currentOffset <= -1) || (currentOffset >= 1))
        {
            currentOffset = Mathf.Sign(currentOffset);
            velocity *= -1;
        }

        material.mainTextureOffset = new Vector2(startOffset.x + range * ((Mathf.Sign(currentOffset) * ((Mathf.Exp(-Mathf.Abs(currentOffset)) - 1) / (Mathf.Exp(-1) - 1)))), startOffset.y);

        //print("[" + currentOffset + "]: " + (Mathf.Sign(currentOffset) * ((Mathf.Exp(- Mathf.Abs(currentOffset)) - 1) / (Mathf.Exp(-1) - 1)))   );
    }
}
