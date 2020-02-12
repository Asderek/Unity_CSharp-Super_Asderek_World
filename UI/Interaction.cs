using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Interaction : MonoBehaviour {

      public Texture bar;
      public string text;
    
    private int fontSize = 17;
    private Color color = Color.white;

      public float xFirstBar, yFirstBar;
      public float xSecondBar, ySecondBar;
      public float xTriangle, yTriangle;
      public float xSwap, ySwap;
      private float xButton, yButton;
      private float xText, yText;
    
     public class DisplayItem {

          public GameObject source;
          public string text;
          public ButtonManager.ButtonID button;

          public DisplayItem(GameObject source, string text, ButtonManager.ButtonID button) {
              this.source = source;
              this.text = text;
              this.button = button;
          }

      }

     DisplayItem item;

    private List<DisplayItem> elements;
    private GUIStyle middleCenter;
    private ButtonManager buttonManager;

    public Area selectionDisplay;
    public Area selectionSecondDisplay;
    public Area selectionSwap;

    public Area selectionText;
    public Area selectionTexture;
    private int secondDisplayIndex = 1;

    void Start() {
        buttonManager = ButtonManager.GetInstance();

        elements = new List<DisplayItem>();
        
        //xFirstBar = Screen.width / 2 - bar.width * 0.6f / 2;
        //yFirstBar = Screen.height * 0.85f;

        //xText = Screen.width / 2 + 10 - bar.width * 0.6f / 2 + buttons[0].width / 10 / 2;
        //yText = yFirstBar;

        //xButton = Screen.width / 2 - 10;
        //yButton = yFirstBar + bar.height * 0.7f / 2 - buttons[0].height / 10 / 2;

        middleCenter = new GUIStyle();
        middleCenter.alignment = TextAnchor.MiddleCenter;
        //middleCenter.fontSize = fontSize;
        //Vector2 size = middleCenter.CalcSize(new GUIContent("Swap"));

        //xSecondBar = Screen.width / 2 - bar.width * 0.6f / 2 + bar.width * 0.015f;
        //ySecondBar = yFirstBar + bar.height * 0.7f * 0.2f;
        //xSwap = Screen.width / 2 - bar.width * 0.6f / 2 + 10 + buttons[0].width / 10 / 2;
        //ySwap = Screen.height * 0.91f;
        //xTriangle = Screen.width / 2 - size.x / 2 - 10;
        //yTriangle = Screen.height * 0.91f + bar.height * 0.7f / 2 - buttons[0].height / 10 / 2;




    }

    public void UpdateOnPlay()
    {
        if (Time.timeScale == 0 || !UIManager.GetInstance().GetPlayer().CanInteract())
            return;

        selectionDisplay.Update();
        selectionSecondDisplay.Update();

        if (elements.Count > 1)
        {
            if (ButtonManager.GetDown(ButtonManager.ButtonID.TRIANGLE,this))
            {
                if (selectionDisplay.GetPercentMove() != 1)
                    return;

                Area.Position sup = new Area.Position(selectionDisplay.centerPosition.x - (selectionSecondDisplay.centerPosition.x - selectionDisplay.centerPosition.x) / 2, selectionDisplay.centerPosition.y - (selectionSecondDisplay.centerPosition.y - selectionDisplay.centerPosition.y) / 2);
                Area.Position inf = new Area.Position(selectionSecondDisplay.centerPosition.x - (selectionDisplay.centerPosition.x - selectionSecondDisplay.centerPosition.x) / 2, selectionSecondDisplay.centerPosition.y - (selectionDisplay.centerPosition.y - selectionSecondDisplay.centerPosition.y) / 2);
                selectionDisplay.GoTo(sup,() =>
                {
                    elements.Add(elements[0]);
                    elements.RemoveAt(0);
                    secondDisplayIndex = elements.Count - 1;
                    selectionDisplay.moveStep /= 2;
                    selectionDisplay.ComeFrom(inf, () => {
                        secondDisplayIndex = 1;
                        selectionDisplay.moveStep *= 2;
                    });
                });
                selectionSecondDisplay.GoTo(inf,() =>
                {
                    selectionSecondDisplay.moveStep /= 2;
                    selectionSecondDisplay.ComeFrom(sup, () => {
                        selectionSecondDisplay.moveStep *= 2;
                    }); 
                });

            }
        }
        middleCenter.fontSize = fontSize;
        middleCenter.normal.textColor = color;
        
        if (elements.Count >= 1)
        {
            while (elements[0].source == null)
            {
                elements.RemoveAt(0);
                if (elements.Count <= 0)
                {
                    return;
                }
            }
            if (elements[0].source.GetComponent<Interactable>() != null)
            {
                if ( Input.GetButtonDown(elements[0].source.GetComponent<Interactable>().button.ToString()))
                {
                    elements[0].source.GetComponent<Interactable>().ActivateInteraction();
                }
            }
        }
            

    }

    public virtual void InitDisplayOnScreen(GameObject source, string text, ButtonManager.ButtonID button)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].source == source)
            {
                return;
            }
        }

        DisplayItem item = new DisplayItem(source, text, button);

        elements.Add(item);

    }

    public virtual void StopDisplayOnScreen(GameObject source)
    {
        //if (item == null)
        //    return;

        //if (item.source == source)
        //    item = null;

        //print((("Stop = " + source);
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].source == source)
            {
                //print((("Cont Before = " + elements.Count);
                elements.Remove(elements[i]);
                //print((("Cont After = " + elements.Count);
                return;
            }
        }
    }

    public void Display()
    {
        Vector2 size;
        Rect rect;

        if (elements.Count > 1)
        {
            GUI.DrawTexture(selectionSecondDisplay.GetRect(), bar, ScaleMode.StretchToFill, true, 0);

            if (selectionSecondDisplay.GetPercentMove() != 1)
            { 
                size = middleCenter.CalcSize(new GUIContent(elements[secondDisplayIndex].text));
                selectionText.SetParent(selectionSecondDisplay);
                selectionTexture.SetParent(selectionSecondDisplay);
                GUI.Label(selectionText.GetRect(), elements[secondDisplayIndex].text, middleCenter);
                rect = selectionTexture.GetRect();
                rect.x -= size.x / 2;
                GUI.DrawTexture(rect, ButtonManager.GetButtonTexture(elements[secondDisplayIndex].button), ScaleMode.StretchToFill, true, 0);
            }

            size = middleCenter.CalcSize(new GUIContent("Swap"));
            selectionText.SetParent(selectionSwap);
            selectionTexture.SetParent(selectionSwap);
            GUI.Label(selectionText.GetRect(), "Swap", middleCenter);
            rect = selectionTexture.GetRect();
            rect.x -= size.x / 2;
            GUI.DrawTexture(rect, ButtonManager.GetButtonTexture(ButtonManager.ButtonID.TRIANGLE), ScaleMode.StretchToFill, true, 0);
        }

        if (elements.Count >= 1)
        {
            GUI.DrawTexture(selectionDisplay.GetRect(), bar, ScaleMode.StretchToFill, true, 0);

            size = middleCenter.CalcSize(new GUIContent(elements[0].text));
            selectionText.SetParent(selectionDisplay);
            selectionTexture.SetParent(selectionDisplay);
            GUI.Label(selectionText.GetRect(), elements[0].text, middleCenter);
            rect = selectionTexture.GetRect();
            rect.x -= size.x/2;
            GUI.DrawTexture(rect, ButtonManager.GetButtonTexture(elements[0].button), ScaleMode.StretchToFill, true, 0);
            
        }
        
    }

    public void ClearScreen()
    {
        elements.Clear();
    }
}
