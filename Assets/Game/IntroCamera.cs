using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        TextMeshProUGUI textScore = GameObject.Find("/Canvas/Score").GetComponent<TextMeshProUGUI>();
        textScore.enabled = true;
        TextMeshProUGUI textFrameRate = GameObject.Find("/Canvas/Framerate").GetComponent<TextMeshProUGUI>();
        textFrameRate.enabled = true;
        TextMeshProUGUI textPlayersLeft = GameObject.Find("/Canvas/PlayersLeft").GetComponent<TextMeshProUGUI>();
        textPlayersLeft.enabled = true;
        TextMeshProUGUI textWeapon = GameObject.Find("/Canvas/Weapon/Description").GetComponent<TextMeshProUGUI>();
        textWeapon.enabled = true;
        Image image = GameObject.Find("/Canvas/Weapon/Image").GetComponent<Image>();
        image.enabled = true;
        image = GameObject.Find("/Canvas/ScoreBackground").GetComponent<Image>();
        image.enabled = true;
        image = GameObject.Find("/Canvas/PlayersLeftBackground").GetComponent<Image>();
        image.enabled = true;
    }
}
