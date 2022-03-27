using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    private AudioSource soundIntro;
    public float delay = 6.4f;
    private Image titleImage;
    private float startingTime;
    private bool isPlaying;

    // Start is called before the first frame update
    void Start()
    {
        titleImage = GetComponent<Image>();
        soundIntro = GameObject.Find("/Sound/Intro").GetComponent<AudioSource>();
    }

    public void ResetTitle()
    {
        startingTime = Time.time;
        titleImage = GetComponent<Image>();
        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        float timePassed = Time.time - startingTime;
        double strength = 0;

        if (timePassed > 2f && !isPlaying)
        {
            titleImage.enabled = true;
            isPlaying = true;
            soundIntro.Play();
        }

        if (timePassed < 5f)
        {
            strength = (timePassed - 2) * 100;
        }
        else if (timePassed > 9f)
        {
            titleImage.enabled = false;
        }
        else
        {
            strength = (8.0 - timePassed) * 100.0;
        }

        if (strength < 0)
        {
            strength = 0;
        }

        if (strength > 255)
        {
            strength = 255;
        }
        titleImage.color = new Color32(255, 255, 225, (byte)strength);
    }
}
