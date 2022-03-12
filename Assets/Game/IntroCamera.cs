using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroCamera : MonoBehaviour
{
    [SerializeField] private GameObject pfPlayer;

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
        TextMeshProUGUI textKills = GameObject.Find("/Canvas/Kills").GetComponent<TextMeshProUGUI>();
        textKills.enabled = true;
        TextMeshProUGUI textFrameRate = GameObject.Find("/Canvas/Framerate").GetComponent<TextMeshProUGUI>();
        textFrameRate.enabled = true;
        TextMeshProUGUI textPlayersLeft = GameObject.Find("/Canvas/TimeLeft").GetComponent<TextMeshProUGUI>();
        textPlayersLeft.enabled = true;
        TextMeshProUGUI textWeapon = GameObject.Find("/Canvas/Weapon/Description").GetComponent<TextMeshProUGUI>();
        textWeapon.enabled = true;
        Image image = GameObject.Find("/Canvas/Weapon/Image").GetComponent<Image>();
        image.enabled = true;
        image = GameObject.Find("/Canvas/ScoreBackground").GetComponent<Image>();
        image.enabled = true;
        image = GameObject.Find("/Canvas/TimeLeftBackground").GetComponent<Image>();
        image.enabled = true;

        // original position: Vector3(19.2399998,38.1899986,89.5899963)
        Vector3 spawnLocation = new Vector3(500 * UnityEngine.Random.value - 250, 1000, 500 * UnityEngine.Random.value - 250);
        Instantiate(pfPlayer, spawnLocation, Quaternion.identity);
    }
}
