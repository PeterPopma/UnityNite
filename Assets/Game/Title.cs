using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    public float delay = 6.4f;
    private Image titleImage;
    private float startingTime;

    // Start is called before the first frame update
    void Start()
    {
        startingTime = Time.time;
        titleImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float timePassed = Time.time - startingTime;
        double strength = 0;

        if (timePassed < 5f)
        {
            strength = (timePassed - 2) * 200;
        }
        else if (timePassed > 9f)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            strength = (8.0 - timePassed) * 128.0;
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