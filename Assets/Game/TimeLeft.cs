using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeLeft : MonoBehaviour
{
    int timeLeft;
    private TextMeshProUGUI textPlayersLeft;
    private float timeLastTimeLeftUpdate;

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = 300; 
        textPlayersLeft = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeLastTimeLeftUpdate + 1.0f)
        {
            timeLastTimeLeftUpdate = Time.time;
            timeLeft--;
            textPlayersLeft.text = "Time left: " + timeLeft.ToString();
        }
    }
}
