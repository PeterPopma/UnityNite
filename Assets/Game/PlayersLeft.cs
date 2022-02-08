using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayersLeft : MonoBehaviour
{
    int playersLeft;
    private TextMeshProUGUI textPlayersLeft;
    private float timeLastPlayerLeftUpdate;

    // Start is called before the first frame update
    void Start()
    {
        playersLeft = 100; 
        textPlayersLeft = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeLastPlayerLeftUpdate + 2.0f)
        {
            timeLastPlayerLeftUpdate = Time.time;
            playersLeft--;
            textPlayersLeft.text = "Players left: " + playersLeft.ToString();
        }
    }
}
