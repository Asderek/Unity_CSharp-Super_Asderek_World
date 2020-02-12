using UnityEngine;
using System.Collections;

[System.Serializable]
public class Drawable
{
    [System.Serializable]
    public enum Behavior
    {
        STATIC,
        FADE,
        PERCENTAGE
    }

    public Texture2D drawTexture;
    public Vector2 offset;
    public Behavior behavior;
    public float lifeTime;
    private int updateCount;
    private Area area;
    public Area.Size size;
    
    [Range(0,1f)]
    public float percent;

    public bool Update()
    {
        updateCount++;

        if (behavior == Behavior.STATIC)
            return false;

        


        if (behavior == Behavior.FADE)
        {
            if (updateCount > lifeTime)
            {
                return true;
            }
        }
        

        return false;
    }

    public Drawable()
    {
        updateCount = 0;
        area = new Area();
    }

    public void Draw(Vector2 position)
    {
        if (drawTexture == null)
            return;
        area.size = size;
        area.centerPosition = GameManager.GetInstance().getCurrentCamera().GetTargetPositionOnScreen(position + offset);

        if (behavior == Behavior.PERCENTAGE)
        {
            Area currentHp = new Area(area);
            currentHp.size.width *= percent;
            //currentHp.centerPosition.x = area.centerPosition.x - area.size.width/2 + currentHp.size.width/2;
            Rect rectText = new Rect(0, 0, percent, 1);

            GUI.DrawTexture(currentHp.GetRect(), drawTexture);
            //GUI.DrawTextureWithTexCoords(currentHp.GetRect(), drawTexture, rectText, true);
        }
        else
            GUI.DrawTexture(area.GetRect(), drawTexture);
    }

}
