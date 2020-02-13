using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace Assets.Scripts
{
    class Utilities
    {
        static public Vector2 standardVector(float a)
        {
            Vector2 standardDirection = new Vector2(a, Mathf.Abs(a));
            standardDirection.Normalize();
            return standardDirection;
        }

        static public Vector2 standardVector(float a, float degree)
        {

            Vector2 standardDirection = new Vector2(a * Mathf.Cos(Mathf.Deg2Rad * degree), Mathf.Abs(a) * Mathf.Sin(Mathf.Deg2Rad * degree));
            standardDirection.Normalize();
            return standardDirection;
        }

        static public bool Intersects(Bounds bound1, Bounds bound2)
        {

            if (bound1.center.x > bound2.center.x)
            {
                if (bound1.center.x - bound1.extents.x > bound2.center.x + bound2.extents.x)
                    return false;
            }
            else
            {

                if (bound1.center.x + bound1.extents.x < bound2.center.x - bound2.extents.x)
                    return false;
            }

            if (bound1.center.y > bound2.center.y)
            {
                if (bound1.center.y - bound1.extents.y > bound2.center.y + bound2.extents.y)
                    return false;
            }
            else
            {
                if (bound1.center.y + bound1.extents.y < bound2.center.y - bound2.extents.y)
                    return false;
            }
            return true;
        }

        static public bool HitAsderek(Collider2D collider) //para trigger
        {
            Transform parent = collider.gameObject.transform.parent;
            if (parent != null)
            {
                if (parent.gameObject.GetComponent<Asderek>() != null)
                {
                    if (!collider.isTrigger)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static public bool HitAsderekAttack(Collider2D collider)
        {
            Transform parent = collider.gameObject.transform.parent;
            if (parent != null)
            {
                if (parent.gameObject.GetComponent<Asderek>() != null)
                {
                    if (collider.isTrigger)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static public bool HitAsderek(Collision2D collision) //collision
        {

            bool ret = HitAsderek(collision.collider);
            return ret;
        }

        internal static float damageFromTo(Commandments.Element attackElement, Commandments.Element myElement)
        {
            if (attackElement == Commandments.Element.NEUTRAL || myElement == Commandments.Element.NEUTRAL)
                return 1;

            return Commandments.damageTable[attackElement.toInt(), myElement.toInt()];
        }
    }

    class GUIMethods
    {
        public static void DrawOverlayTexture(Rect area, Texture2D baseText, Texture2D overlayText, float percent, ActiveAbilitiesMenu.Options direction, ScaleMode scaleMode = ScaleMode.ScaleToFit)
        {

            if (baseText != null)
                GUI.DrawTexture(area, baseText, scaleMode);

            Rect rectText = new Rect();
            switch (direction)
            {
                case ActiveAbilitiesMenu.Options.UP:
                    area.height *= percent;
                    rectText = new Rect(0, 1-percent, 1, percent);
                    break;
                case ActiveAbilitiesMenu.Options.DOWN:
                    area.y += area.height * (1 - percent);
                    area.height *= percent;
                    rectText = new Rect(0, 0, 1, percent);
                    break;
                case ActiveAbilitiesMenu.Options.LEFT:
                    area.width *= percent;
                    rectText = new Rect(0, 0, percent, 1);
                    break;
                case ActiveAbilitiesMenu.Options.RIGHT:
                    area.x += area.width * (1 - percent);
                    area.width *= percent;
                    rectText = new Rect(1 - percent, 0, percent, 1);
                    break;
            }
            
            if (overlayText != null)
                GUI.DrawTextureWithTexCoords(area, overlayText, rectText, true);
            
        }
    }

}
