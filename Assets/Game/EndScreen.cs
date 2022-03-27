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
    private string mostKills = "";
    private string highestScore = "";
    private string mostAccurate = "";

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
        StartCoroutine(CalculateScores());
        textEndScreen.text = "Game has ended!\n" + "Your accuracy: " + GameManager.Instance.Player.GetComponent<Player>().GetAccuracy().ToString("0.00") + " %\n";
        textEndScreen.text += "\n";
        if (mostKills.Length > 0)
        {
            textEndScreen.text += "Most kills: " + mostKills + "\n";
        }
        if (highestScore.Length > 0)
        {
            textEndScreen.text += "Highest score: " + highestScore + "\n";
        }
        if (mostAccurate.Length > 0)
        {
            textEndScreen.text += "Most accurate player: " + mostAccurate + "\n";
        }
        int timeLeft = (int)(20 - (Time.time - startTime));
        textEndScreenTime.text = "Next game starting in " + timeLeft;
        if(timeLeft < 0)
        {
            highestScore = "";
            mostAccurate = "";
            mostKills = "";
            ResetGameTime();
            GameManager.Instance.UpdateGameState(GameState.Intro);
        }
    }

    IEnumerator CalculateScores()
    {
        // Wait some time to send the players scores over the network
        yield return new WaitForSeconds(2);

        float highest = 0;
        var players = FindObjectsOfType<Player>();

        foreach (Player player in players)
        {
            if(player.Score > highest)
            {
                highest = player.Score;
                highestScore = player.DisplayName + " (" + player.Score.ToString() + ")";
            }
            highest = 0;
            if (player.Kills > highest)
            {
                highest = player.Kills;
                mostKills = player.DisplayName + " (" + player.Kills.ToString() + ")";
            }
            highest = 0;
            if (player.GetAccuracy() > highest)
            {
                highest = player.GetAccuracy();
                mostAccurate = player.DisplayName + " (" + player.GetAccuracy().ToString("0.00") + " %)";
            }
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
