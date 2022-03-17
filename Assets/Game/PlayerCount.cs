using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerCount : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textPlayers;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            textPlayers.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }
}
