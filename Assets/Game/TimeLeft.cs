using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TimeLeft : MonoBehaviour
{
    private TextMeshProUGUI textTimeLeft;
    double startTime;

    // Start is called before the first frame update
    void Start()
    {
        textTimeLeft = GetComponent<TextMeshProUGUI>();
    }

    public void InitTimer()
    {
        startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.gameState.Equals(GameState.Game))
        {
            return;
        }

        int timeLeft = (int)(30 - (PhotonNetwork.Time - startTime));
        textTimeLeft.text = "Time left: " + timeLeft.ToString();

        if (timeLeft <= 0)
        {
            GameManager.Instance.UpdateGameState(GameState.Ended);
        }
    }
}
