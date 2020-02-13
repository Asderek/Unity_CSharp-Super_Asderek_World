using UnityEngine;
using System.Collections;

public class ObjectFader : MonoBehaviour
{
    float Delay;
    Color colorBegin;
    Color colorEnd;

    void Start()
    {
        colorBegin = GetComponentInChildren<SpriteRenderer>().color;
        //colorBegin.r *= 0.3f;
        //colorBegin.g *= 0.3f;
        //colorBegin.b *= 0.3f;

        colorEnd = new Color(0, 0, 0, 0f);
    }

    void Update()
    {
            Delay += Time.deltaTime;
            GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(colorBegin, colorEnd, Delay * 4f);

             if (transform.GetComponentInChildren<SpriteRenderer>().color.a == 0)
            {
                GameObject.Destroy(gameObject);
            }
        
    }
}