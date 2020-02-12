using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to represent a basic abstraction of a minimal Menu showed in circle
/// </summary>
[System.Serializable]
public abstract class ClockMenu : AbstractMenu
{

    [Tooltip("Whole menu area")]
    public Area elementArea;

    [Tooltip("Radius of the circle inside menu area")]
    [Range (0f,1f)]
    public float radius;

    [Tooltip("Angle of the first element")]
    [Range (0f,360f)]
    public float startAngle;

    [Tooltip("Relative center of the menu")]
    public Area.Position center;

    List<Area.Position> positions = new List<Area.Position>();

    /// <summary>
    /// Function to initialize some internal properties. Should be called in Start()
    /// </summary>
    /// <param name="Index">Index to control the menu index</param>
    public override void Init(LimitedIndex Index)
    {
        base.Init(Index);
        elementArea.SetParent(menuArea);

        for (int i = 0; i < menuIndex.Max(); i++)
        {
            float angle = startAngle - (360 / (float)menuIndex.Max()) * i;
            positions.Add(new Area.Position(center.x + (Mathf.Cos(Mathf.Deg2Rad * angle) * radius / 2.0f), center.y + (-Mathf.Sin(Mathf.Deg2Rad * angle) * radius / 2.0f)));
        }





        //for (int k=0; k<100;k++)
        //{
        //    List<Area.Position> newPositions = new List<Area.Position>();

        //    CircularIndex index = new CircularIndex(positions.Count);
        //    for (int i = 0; i < positions.Count; i++)
        //    {
        //        float x_center = (positions[index.IndexMovedBy(-1)].x + positions[index.IndexMovedBy(1)].x) / 2;
        //        newPositions.Add(new Area.Position(x_center, positions[index.Get()].y));
        //        index++;
        //    }
        //    positions = newPositions;
        //}

    }

    /// <summary>
    /// Function to calcule the position of each visible element and call DrawElement
    /// Should be call in OnGui when the menu has to be draw
    /// </summary>
    /// <see cref="DrawElement(Rect, int, bool)"/>
    public override void Draw()
    {
        DrawContext();

        if (menuIndex == null)
            return;

        if (menuIndex.Max() == 0)
            return;

        for (int i = 0; i < menuIndex.Max(); i++)
        {
            float angle = startAngle - (360 / (float)menuIndex.Max()) * i;
            elementArea.centerPosition = new Area.Position(center.x + (Mathf.Cos(Mathf.Deg2Rad * angle) * radius / 2.0f), center.y + (-Mathf.Sin(Mathf.Deg2Rad * angle) * radius / 2.0f));
            DrawElement(elementArea.GetRect(), i, menuIndex.Get() == i);
        }
        //int i = 0;
        //foreach (Area.Position position in positions)
        //{
        //    elementArea.centerPosition = position;
        //    DrawElement(elementArea.GetRect(), i, menuIndex.Get() == i);
        //    i++;
        //}

        //positions.Clear();
        //float e2 = 1 - Mathf.Pow((menuArea.size.height / menuArea.size.width), 2);
        //for (i = 0; i < menuIndex.Max(); i++)
        //{
        //    float t = i * (2 * Mathf.PI) / menuIndex.Max();
        //    float teta = t - (e2 / 8 + Mathf.Pow(e2, 2) / 16 + 71 * Mathf.Pow(e2, 3) / 2048) * Mathf.Sin(2 * t)
        //        + (5 * Mathf.Pow(e2, 2) / 256 + 5 * Mathf.Pow(e2, 3) / 256) * Mathf.Sin(4 * t)
        //        + (29 * Mathf.Pow(e2, 3) / 6144) * Mathf.Sin(6 * t)
        //        + (Mathf.Exp(Mathf.Pow(e2, 4)) - 1);

        //    positions.Add(new Area.Position(menuArea.size.width * Mathf.Cos(teta), menuArea.size.height * Mathf.Sin(teta)));
        //}
    }

    public override Area GetArea(int element)
    {
        if (element >= menuIndex.Max())
            return new Area(0, 0, 1, 1);

        float angle = startAngle - (360 / (float)menuIndex.Max()) * menuIndex.Get();
        elementArea.centerPosition = new Area.Position(center.x + (Mathf.Cos(Mathf.Deg2Rad * angle) * radius / 2.0f), center.y + (-Mathf.Sin(Mathf.Deg2Rad * angle) * radius / 2.0f));

        return new Area(elementArea.GetRect());
    }

}
