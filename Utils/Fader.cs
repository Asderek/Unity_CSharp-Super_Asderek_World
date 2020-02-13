using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour
{

    public Texture2D fadeOutTexture;    //the texture that will overlay the screen, prbly a black image
    public float fadeSpeed = 0.8f;      //fading speed

    private int drawDepth = -1000;  //the textures order in the draw hierarchy: a low number means it renders on top
    private float alpha = 1.0f;     //texture alpha [0,1]
    private int fadeDir = -1;       //the direction to fade: if -1 then fadein if 1 then fadeout

    void OnGUI()
    {
        // fade out/in the alpha values using a direction, a speed and Time.deltatime to cnvert the operations to seconds
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        // force (clamp) the number between 0 and 1 because GUI.color uses alpha [0,1]
        alpha = Mathf.Clamp01(alpha);

        // set color of GUI. All color values remain the same & the alpha is variable
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);        //set alpha variable
        GUI.depth = drawDepth;                                                      //make the black texture render on top of the screen
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);                //make the texture fill the entire screen

    }

    // sets fadeDir to the direction parameter making the scene fade in if -1 and out if 1
    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);     // return the fadeSpeed var so it's easy to time the Application.LoadLevel(); //time to fade
    }

    //OnLevelWasLoaded is called when a level is loaded. It takes loaded level index (int) as a parameter so you can limit the fade in to certains scenes.
    void OnLevelWasLoaded()
    {
        BeginFade(-1);      //call the fade funciotn
    }


}
