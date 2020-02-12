using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CardinalTextures
{
    public enum Position
    {
        NORTH,
        SOUTH,
        EAST,
        WEST,
        N_WEST,
        N_EAST,
        S_WEST,
        S_EAST,
        CENTER
    }
    
    [System.Serializable]
    public class Textures : SerializableDictionary<Position, Texture2D> { }

    public Textures textures;
    public Area.Position offset;
    public Area.Size size;

    private Dictionary<Position, Area> areas = new Dictionary<Position, Area>();

    public void Draw()
    {
        Draw(new Area(0.5f,0.5f,1,1));
    }

    public void UpdateAreas()
    {
        foreach (Position pos in Enum.GetValues(typeof(Position)))
        {
            switch (pos)
            {
                case Position.NORTH:
                    areas[pos] = new Area(0.5f, offset.y+size.height/2, size.width, size.height);
                    break;
                case Position.SOUTH:
                    areas[pos] = new Area(0.5f, 1 - (offset.y + size.height / 2), size.width, size.height);
                    break;
                case Position.EAST:
                    areas[pos] = new Area(1-(offset.x + size.width / 2), 0.5f, size.width, size.height);
                    break;
                case Position.WEST:
                    areas[pos] = new Area(offset.x + size.width / 2, 0.5f, size.width, size.height);
                    break;
                case Position.N_EAST:
                    areas[pos] = new Area(1 - (offset.x + size.width / 2), offset.y + size.height / 2, size.width, size.height);
                    break;
                case Position.N_WEST:
                    areas[pos] = new Area(offset.x + size.width / 2, offset.y + size.height / 2, size.width, size.height);
                    break;
                case Position.S_EAST:
                    areas[pos] = new Area(1 - (offset.x + size.width / 2), 1 - (offset.y + size.height / 2), size.width, size.height);
                    break;
                case Position.S_WEST:
                    areas[pos] = new Area(offset.x + size.width / 2, 1 - (offset.y + size.height / 2), size.width, size.height);
                    break;
                case Position.CENTER:
                    areas[pos] = new Area(0.5f, 0.5f, size.width, size.height);
                    break;
            }
        }
    }

    public void Draw(Area parent)
    {
        UpdateAreas();
        foreach (KeyValuePair<Position, Texture2D> element in textures)
        {
            if (element.Value)
            {
                GUI.DrawTexture(areas[element.Key].GetRect(parent.GetRect()),element.Value);
            }
        }


    }


}