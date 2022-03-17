using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    double startTime; 
    private TextMeshProUGUI textEndScreenTime;
    private TextMeshProUGUI textEndScreen;

    // Start is called before the first frame update
    void Start()
    {
        textEndScreenTime = GameObject.Find("/Canvas/EndScreen/TextTime").GetComponent<TextMeshProUGUI>();
        textEndScreen = GameObject.Find("/Canvas/EndScreen/Text").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.gameState.Equals(GameState.Ended))
        {
            return;
        }
        textEndScreen.text = "Game has ended!\n" + "Your accuracy: " + GameManager.Instance.Player.GetComponent<Player>().GetAccuracy().ToString("0.00") + " %\n";
        textEndScreen.text += "\n";
        textEndScreen.text += "Most kills: \n";
        textEndScreen.text += "Highest score: \n";
        textEndScreen.text += "Most accurate player: \n";
        int timeLeft = (int)(10 - (Time.time - startTime));
        textEndScreenTime.text = "Next game starting in " + timeLeft;
        if(timeLeft < 0)
        {
            ResetGameTime();
            GameManager.Instance.UpdateGameState(GameState.Intro);
        }
    }

    public void ResetEndScreenTime()
    {
        startTime = Time.time;
    }

    private void ResetGameTime()
    {
        ExitGames.Client.Photon.Hashtable CustomValues = new ExitGames.Client.Photon.Hashtable();
        CustomValues.Add("StartTime", PhotonNetwork.Time);
        PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValues);
    }
}
