using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    private bool joinedNetworkGame;
    public bool JoinedNetworkGame { get => joinedNetworkGame; set => joinedNetworkGame = value; }
    private GameObject player;
    [SerializeField] private GameObject pfPlayer;

    // Start is called before the first frame update
    void Start()
    {
        joinedNetworkGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState.Equals(GameState.InitGame) && joinedNetworkGame)
        {
            Vector3 spawnLocation = new Vector3(1000 * UnityEngine.Random.value - 500, 700, 1000 * UnityEngine.Random.value - 500);
            player = PhotonNetwork.Instantiate(pfPlayer.name, spawnLocation, Quaternion.identity);
            int topClothes = UnityEngine.Random.Range(1, 10);
            int bottomClothes = UnityEngine.Random.Range(1, 10);
            Player scriptPlayer = player.GetComponent<Player>();
            scriptPlayer.PhotonView.RPC("ChangeClothes", RpcTarget.All, scriptPlayer.PhotonView.ViewID, topClothes, bottomClothes);
            GameManager.Instance.Player = player;
            GameManager.Instance.UpdateGameState(GameState.Intro);
        }
    }
}
