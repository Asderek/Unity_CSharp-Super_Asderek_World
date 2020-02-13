using UnityEngine;
using System.Collections;

public class Blink : MonoBehaviour {

    float Delay = 0;
    private float step=0.2f;
    Color colorBegin;
    Color colorEnd;
    GameObject target;
    bool play = false;
    Color OriginalColor;

    void Start()
    {
        colorBegin = GetComponentInChildren<SpriteRenderer>().color;
        OriginalColor = colorBegin;
        colorEnd = colorBegin;
        colorEnd.a = 0f;
    }

    void Update()
    {
        if (play == true)
        {
            Delay += step;
            GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(colorBegin, colorEnd, Delay);

            if (Delay>=1)
            {
                Color colorAux;
                colorAux = colorBegin;
                colorBegin = colorEnd;
                colorEnd = colorAux;
                Delay = 0;
            }
        }

    }

    public void Play()
    {
        play = true;
    }

    public void Stop()
    {
        play = false;
        GetComponentInChildren<SpriteRenderer>().color = OriginalColor;
    }

}
