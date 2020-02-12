using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to make some texture blink
/// </summary>
public class BlinkTexture
{

    /// <summary>
    /// Class to dynamically controls the blink of a texture despite changes in textures selection
    /// </summary>
    [System.Serializable]
    public class BlinkControl
    {
        [Tooltip ("Increment step. The Whiteness will variate from min to max")]
        [Range (0.01f,0.2f)]
        public float speed = 0.02f;

        [Tooltip("Minimal Whiteness from 0(normal image) to 1(all white)")]
        [Range(0f, 1f)]
        public float minWhiteness = 0.1f;
        [Tooltip("Maximal Whiteness from 0(normal image) to 1(all white)")]
        [Range(0f, 1f)]
        public float maxWhiteness = 0.5f;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkControl"/> class.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <param name="minWhiteness">The minimum whiteness.</param>
        /// <param name="maxWhiteness">The maximum whiteness.</param>
        public BlinkControl(float speed, float minWhiteness, float maxWhiteness)
        {
            this.speed = speed;
            this.minWhiteness = minWhiteness;
            this.maxWhiteness = maxWhiteness;
        }
    }

    private Texture2D internalText;
    private Texture2D auxText;
    
    private float whitenessLevels;
    private BlinkControl blinkControl;
    private float factor;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlinkTexture"/> class.
    /// </summary>
    /// <param name="baseTexture">The base texture.</param>
    /// <param name="blinkControl">The reference to a struct to control the blink</param>
    public BlinkTexture(Texture2D baseTexture, BlinkControl blinkControl)
    {
        whitenessLevels = blinkControl.minWhiteness;
        factor = 1;
        this.blinkControl = blinkControl;
        this.internalText = baseTexture;
        auxText = new Texture2D(internalText.width, internalText.height);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BlinkTexture"/> class.
    /// </summary>
    /// <param name="baseTexture">The base texture.</param>
    /// <param name="speed">The speed.</param>
    /// <param name="minWhiteness">The minimum whiteness.</param>
    /// <param name="maxWhiteness">The maximum whiteness.</param>
    public BlinkTexture(Texture2D baseTexture, float speed = 0.015f, float minWhiteness = 0, float maxWhiteness = 0.4f):
        this(baseTexture, new BlinkControl(speed,minWhiteness,maxWhiteness))
    {
    }

    /// <summary>
    /// Should be called in each update where the texture should blink.
    /// </summary>
    public void Update()
    {
        if (blinkControl.minWhiteness > blinkControl.maxWhiteness)
        {
            float aux = blinkControl.minWhiteness;
            blinkControl.minWhiteness = blinkControl.maxWhiteness;
            blinkControl.maxWhiteness = aux;
        }


        if (whitenessLevels < blinkControl.minWhiteness) 
        {
            factor = 1;
        } else if (whitenessLevels > blinkControl.maxWhiteness)
        {
            factor = -1;
        }

        whitenessLevels += factor*blinkControl.speed;
    }

    /// <summary>
    /// Restore the minWhiteness level
    /// </summary>
    public void Reset()
    {
        whitenessLevels = blinkControl.minWhiteness;
    }

    /// <summary>
    /// Gets the texture to draw.
    /// </summary>
    /// <returns></returns>
    public Texture2D GetTexture()
    {

        Color[] colorArray = new List<Color>(internalText.GetPixels()).ToArray();

        for (int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i].r += whitenessLevels;
            colorArray[i].b += whitenessLevels;
            colorArray[i].g += whitenessLevels;
        }
        auxText.SetPixels(colorArray);
        auxText.Apply();
        return auxText;
    }
}
