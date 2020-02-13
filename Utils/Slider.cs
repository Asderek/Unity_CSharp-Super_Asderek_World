using UnityEngine;

[System.Serializable]
public class Slider
{
    public Texture2D baseBar;
    public Texture2D fillBar;
    public Texture2D marker;
    [Range(0,1f)]
    public float markerSize;
    [Range(0, 1f)]
    public float baseHeight;


    public void Display(Rect rect, float value, float min, float max) {

        rect.y += rect.height / 2;
        rect.height *= baseHeight;
        rect.y -= rect.height / 2;

        GUI.DrawTexture(rect, baseBar, ScaleMode.StretchToFill, true, 0);

        if (value != min)
        { 
            rect.width *= (value - min) / (max - min);
            Rect rectFill = new Rect(0, 0, (value - min) / (max - min), 1);
            GUI.DrawTextureWithTexCoords(rect, fillBar, rectFill);
            rect.x += rect.width;
            rect.width /= (value - min) / (max - min);
        }
        rect.width *= markerSize;
        if (((rect.width * marker.height) / (float)marker.width) > rect.height)
        {
            rect.width = (rect.height * marker.width) / (float)marker.height;
        }
        else
        {
            rect.height = (rect.width * marker.height) / (float)marker.width;
        }

        rect.x -= rect.width/2;
        GUI.DrawTexture(rect, marker, ScaleMode.ScaleToFit);

    }
}