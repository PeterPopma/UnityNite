using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonServer : MonoBehaviourPunCallbacks
{
    private InitGame initGame;
    private ExitGames.Client.Photon.Hashtable CustomValues;

    // Start is called before the first frame update
    void Start()
    {
        initGame = GameObject.Find("/Scripts/InitGame").GetComponent<InitGame>();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()    
    {
        PhotonNetwork.JoinLobby();        
    }

    public override void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions();
        PhotonNetwork.JoinOrCreateRoom("FartniteGame", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.CurrentRoom.CustomProperties["StartTime"] == null)
        {
            CustomValues = new ExitGames.Client.Photon.Hashtable();
            CustomValues.Add("StartTime", PhotonNetwork.Time);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValues);
        }

        initGame.JoinedNetworkGame = true;
    }
}
