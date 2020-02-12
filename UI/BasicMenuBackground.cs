using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

[System.Serializable]
public class BasicMenuBackground {

    public Texture2D background;
    public Texture2D north;
    public Texture2D south;
    public Texture2D west;
    public Texture2D east;

    public Area area;

    public void Draw()
    {
        GUI.DrawTexture(area.GetRect(), background, ScaleMode.StretchToFill, true, 0);
        GUI.DrawTexture(area.GetRect(), north, ScaleMode.ScaleToFit, true, 0);
        GUI.DrawTexture(area.GetRect(), south, ScaleMode.ScaleToFit, true, 0);
        GUI.DrawTexture(area.GetRect(), west, ScaleMode.ScaleToFit, true, 0);
        GUI.DrawTexture(area.GetRect(), east, ScaleMode.ScaleToFit, true, 0);
    }

}
