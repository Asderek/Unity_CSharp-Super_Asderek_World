using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance = null;

    public AudioClip testSound;
    private AudioSource backgroundMusic;

    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);


        
        backgroundMusic = gameObject.AddComponent<AudioSource>();
        backgroundMusic.clip = testSound;
        backgroundMusic.loop = true;
        backgroundMusic.Play();

    }



}
