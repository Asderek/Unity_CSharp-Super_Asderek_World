using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DrawableManager
{
    public List<Drawable> drawList;

    public void FixedUpdate()
    {
        for (int i = 0; i < drawList.Count; i++)
        {
            if (drawList[i].Update())
            {
                drawList.Remove(drawList[i]);
                i--;
            }
        }
    }

    public void OnGUI(Vector2 position)
    {
        foreach (Drawable drawable in drawList)
        {
            drawable.Draw(position);
        }

        

    }

}
